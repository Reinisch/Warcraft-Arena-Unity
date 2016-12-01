using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using BasicRpgEngine.Physics;
using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Characters
{
    public class NetworkPlayer : ITargetable
    {
        public int ID
        { get; private set; }
        public Color Team
        { get; set; }
        public bool IsDead
        { get; set; }
        public bool IsHumanPlayer
        { get; private set; }
        public Character Character
        { get; set; }
        public GameMap GameMapRef
        { get; set; }
        public bool NeedsDispose
        { get; private set; }
        private Vector2 playerInput = Vector2.Zero;
        private Vector2 playerNewInput = Vector2.Zero;
        private TimeSpan moveTimer = TimeSpan.Zero;

        NetworkPlayerState simulationState;
        NetworkPlayerState previousState;
        NetworkPlayerState displayState;
        float currentSmoothing;
        RollingAverage clockDelta = new RollingAverage(100);

        public bool IsFlying
        {
            get { return displayState.IsFlying; }
            set { displayState.IsFlying = value; }
        }
        public bool IsGrounded
        {
            get { return displayState.IsGrounded; }
            set { displayState.IsGrounded = value; }
        }
        public bool IsKnockedBack
        {
            get { return displayState.IsKnockedBack; }
            set { displayState.IsKnockedBack = value; }
        }
        public BoundRectangle BoundRect
        {
            get { return displayState.BoundRect; }
            set { displayState.BoundRect = value; }
        }
        public Vector2 Position
        {
            get { return displayState.Position; }
            set { displayState.Position = value; }
        }
        public Vector2 Velocity
        {
            get { return displayState.Velocity; }
            set { displayState.Velocity = value; }
        }

        public float PlayerSpeed
        { get { return Character.Entity.Speed; } }

        public NetworkPlayer(int id, Color team, Character character,GameMap gameMapRef)
        {
            ID = id;
            Team = team;
            IsDead = false;
            IsHumanPlayer = true;
            Character = character;
            GameMapRef = gameMapRef;
            NeedsDispose = false;

            simulationState = new NetworkPlayerState(new BoundRectangle(0, 0, Character.Sprite.Width / 3, Character.Sprite.Height),
                Vector2.Zero, true, false, false, Vector2.Zero);
            previousState = (NetworkPlayerState)simulationState.Clone();
            displayState = (NetworkPlayerState)simulationState.Clone();
        }

        public void AlignStates()
        {
            simulationState = (NetworkPlayerState)displayState.Clone();
            previousState = (NetworkPlayerState)displayState.Clone();
        }
        public void UpdateLocal(Vector2 newPlayerInput, GameTime gameTime)
        {
            if (Character.Entity.IsStunned || Character.Entity.IsRooted || IsKnockedBack)
                playerInput = Vector2.Zero;
            else if (Character.Entity.IsPolymorphed)
            {
                playerInput.Y = 0;
                moveTimer -= gameTime.ElapsedGameTime;

                if (moveTimer.TotalSeconds < 0)
                {
                    moveTimer = new TimeSpan(0, 0, 0, 0, 400);
                    int x = Mechanics.Roll(0, 7);
                    switch (x)
                    {
                        case 1:
                            playerInput.X = PlayerSpeed / 2;
                            break;
                        case 2:
                            playerInput.X = -PlayerSpeed / 2;
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            playerInput.X = 0;
                            break;
                    }
                }
            }
            else if (Character.Entity.IsNoControlable)
                playerInput = Vector2.Zero;
            else
                playerInput = newPlayerInput;

            UpdateState(simulationState);

            displayState = (NetworkPlayerState)simulationState.Clone();
        }
        public void UpdateClientInHost(int framesBetweenPackets, bool enablePrediction, bool needSpeedCorrection, GameTime gameTime)
        {
            if (Character.Entity.IsStunned || Character.Entity.IsRooted || IsKnockedBack)
                playerInput = Vector2.Zero;
            else if (Character.Entity.IsPolymorphed)
            {
                playerInput.Y = 0;
                moveTimer -= gameTime.ElapsedGameTime;

                if (moveTimer.TotalSeconds < 0)
                {
                    moveTimer = new TimeSpan(0, 0, 0, 0, 400);
                    int x = Mechanics.Roll(0, 7);
                    switch (x)
                    {
                        case 1:
                            playerInput.X = PlayerSpeed / 2;
                            break;
                        case 2:
                            playerInput.X = -PlayerSpeed / 2;
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            playerInput.X = 0;
                            break;
                    }
                }
            }
            else if (Character.Entity.IsNoControlable)
                playerInput = Vector2.Zero;
            else
                playerInput = playerNewInput;

            float smoothingDecay = 1.0f / framesBetweenPackets;

            currentSmoothing -= smoothingDecay;

            if (currentSmoothing < 0)
                currentSmoothing = 0;

            if (enablePrediction)
            {
                UpdateState(simulationState);

                // If both smoothing and prediction are active,
                // also apply prediction to the previous state.
                if (currentSmoothing > 0)
                {
                    UpdateState(previousState);
                }
            }

            if (currentSmoothing > 0)
            {
                // Interpolate the display state gradually from the
                // previous state to the current simultation state.
                ApplySmoothing(needSpeedCorrection);
            }
            else
            {
                // Copy the simulation state directly into the display state.
                displayState = (NetworkPlayerState)simulationState.Clone();
            }
        }
        public void UpdateInClient(int framesBetweenPackets, bool enablePrediction, bool needSpeedCorrection, GameTime gameTime)
        {
            float smoothingDecay = 1.0f / framesBetweenPackets;

            currentSmoothing -= smoothingDecay;

            if (currentSmoothing < 0)
                currentSmoothing = 0;

            if (enablePrediction)
            {
                UpdateState(simulationState);

                // If both smoothing and prediction are active,
                // also apply prediction to the previous state.
                if (currentSmoothing > 0)
                {
                    UpdateState(previousState);
                }
            }

            if (currentSmoothing > 0)
            {
                // Interpolate the display state gradually from the
                // previous state to the current simultation state.
                ApplySmoothing(needSpeedCorrection);
            }
            else
            {
                // Copy the simulation state directly into the display state.
                displayState = (NetworkPlayerState)simulationState.Clone();
            }
        }
        public void WriteNetworkPacket(PacketWriter packetWriter, GameTime gameTime, byte id)
        {
            packetWriter.Write('M');
            packetWriter.Write(id);
            // Send our current time.
            packetWriter.Write((float)gameTime.TotalGameTime.TotalSeconds);

            // Send the current state.
            packetWriter.Write(simulationState.Position);
            packetWriter.Write(simulationState.Velocity);

            // Also send our current inputs. These can be used to more accurately
            // predict how to move in the future.
            packetWriter.Write(playerInput);
        }
        public void ReadNetworkPacketFromServer(PacketReader packetReader, GameTime gameTime, TimeSpan latency, bool enablePrediction, bool enableSmoothing)
        {
            if (enableSmoothing)
            {
                previousState = (NetworkPlayerState)displayState.Clone();
                currentSmoothing = 1;
            }
            else
            {
                currentSmoothing = 0;
            }

            float packetSendTime = packetReader.ReadSingle();

            simulationState.Position = packetReader.ReadVector2();
            simulationState.Velocity = packetReader.ReadVector2();
            playerInput = packetReader.ReadVector2();

            if (enablePrediction)
            {
               ApplyPrediction(gameTime, latency, packetSendTime);
            }
        }
        public void ReadNetworkPacketFromClient(PacketReader packetReader,GameTime gameTime, TimeSpan latency,bool enablePrediction, bool enableSmoothing)
        {
            if (enableSmoothing)
            {
                previousState = (NetworkPlayerState)displayState.Clone();
                currentSmoothing = 1;
            }
            else
            {
                currentSmoothing = 0;
            }

            float packetSendTime = packetReader.ReadSingle();

            playerNewInput = packetReader.ReadVector2();
        }
        void ApplyPrediction(GameTime gameTime, TimeSpan latency, float packetSendTime)
        {
            float localTime = (float)gameTime.TotalGameTime.TotalSeconds;
            float timeDelta = localTime - packetSendTime;

            clockDelta.AddValue(timeDelta);

            float timeDeviation = timeDelta - clockDelta.AverageValue;
            latency += TimeSpan.FromSeconds(timeDeviation);
            TimeSpan oneFrame = TimeSpan.FromSeconds(1.0 / 60.0);

 
            while (latency >= oneFrame)
            {
                UpdateState(simulationState);
                latency -= oneFrame;
            }
        }
        void ApplySmoothing(bool needSpeedCorrection)
        {
            displayState.Position = Vector2.Lerp(simulationState.Position,
                                     previousState.Position,
                                     currentSmoothing);
            displayState.Velocity = Vector2.Lerp(simulationState.Velocity,
                                     previousState.Velocity,
                                     currentSmoothing);
            if (needSpeedCorrection)
            {
                displayState.Velocity = new Vector2((Math.Abs(displayState.Velocity.X) < 0.05f) ? 0 : displayState.Velocity.X,
                    (Math.Abs(displayState.Velocity.Y) < 0.05f) ? 0 : displayState.Velocity.Y);
            }
        }
        void UpdateState(NetworkPlayerState state)
        {
            if (state.IsGrounded)
                state.Velocity = playerInput;

            state.IsGrounded = false;
            for (int i = 0; i < GameMapRef.MapObjects.Length; i++)
            {
                GameMapRef.MapObjects[i].CheckCollision(state, playerInput);
            }

            state.Position += state.Velocity;
            state.Position = Vector2.Clamp(state.Position, new Vector2(100, -100), new Vector2(GameMapRef.MapWidth - 200, GameMapRef.MapHeight + 300));

            if (!state.IsGrounded)
            {
                state.Velocity += new Vector2(0, 0.3f);
            }
            else
            {
                state.IsKnockedBack = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Character.Draw(spriteBatch, BoundRect);
        }

        public void Dispose()
        {
            Character.Dispose();
            GameMapRef = null;
            NeedsDispose = true;
        }
    }
}
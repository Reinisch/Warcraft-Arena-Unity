using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BasicRpgEngine.Physics;
using BasicRpgEngine.Characters.AI;

namespace BasicRpgEngine.Characters
{
    public class Creature : ITargetable, IDisposable
    {
        public int ID
        { get; private set; }
        public Color Team { get; set; }
        public bool IsDead
        { get; set; }
        public bool IsHumanPlayer
        { get; private set; }
        private BoundRectangle boundRect;
        private Vector2 position;
        public ICreatureStrategy CombatStrategy { get; set; }
        public ICreatureStrategy PathStrategy { get; set; }

        public Vector2 playerInput;
        public TimeSpan MoveTimer { get; set; }
        public TimeSpan CastTimer { get; set; }
        public Character Character { get; private set; }
        public GameMap GameMapRef { get; private set; }
        public Vector2 Velocity { get; set; }
        public bool IsFlying { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsKnockedBack { get; set; }
        public bool NeedsDispose { get; set; }
        public bool NeedsEscapeToRight { get; set; }
        public bool NeedsEscapeToLeft { get; set; }
        public bool NeedsElevation { get; set; }
        public Vector2 ElevationDestination { get; set; }

        public BoundRectangle BoundRect
        {
            get { return boundRect; }
            set { boundRect = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                boundRect.X = position.X + BoundRect.Width;
                boundRect.Y = position.Y;
            }
        }
        public float PlayerSpeed
        { get { return Character.Entity.Speed; } }

        
        public Creature(Color team, ICreatureStrategy newCombatStrategy, ICreatureStrategy newPathStrategy, Character character, GameMap gameMapRef)
        {
            ID = 0;
            Team = team;
            CombatStrategy = newCombatStrategy;
            PathStrategy = newPathStrategy;
            IsDead = false;
            IsHumanPlayer = false;
            GameMapRef = gameMapRef;
            MoveTimer = TimeSpan.Zero;
            CastTimer = TimeSpan.FromSeconds(Mechanics.Roll(4,30));
            playerInput = Vector2.Zero;
            Character = character;
            Position = new Vector2(300, 300);
            Velocity = Vector2.Zero;
            boundRect.Width = Character.Sprite.Width/3;
            boundRect.Height = Character.Sprite.Height;
            IsFlying = false;
            IsGrounded = true;
            IsKnockedBack = false;
            NeedsDispose = false;
            ElevationDestination = Vector2.Zero;
        }

        public void Update(TimeSpan elapsedGameTime, World world)
        {
            PathStrategy.ApplyStrategy(this, world);
            CastTimer -= elapsedGameTime;
            MoveTimer -= elapsedGameTime;

            if (Character.Entity.IsPolymorphed)
            {
                playerInput.Y = 0;
                MoveTimer -= elapsedGameTime;

                if (MoveTimer.TotalSeconds < 0)
                {
                    MoveTimer = new TimeSpan(0, 0, 0, 0, 400);
                    int x = Mechanics.Roll(0, 7);
                    switch(x)
                    {
                        case 1:
                            playerInput.X = PlayerSpeed/2;
                            break;
                        case 2:
                            playerInput.X = -PlayerSpeed/2;
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
            else if (Character.Entity.IsFeared)
            {
                playerInput.Y = 0;
                MoveTimer -= elapsedGameTime;

                if (MoveTimer.TotalSeconds < 0)
                {
                    MoveTimer = new TimeSpan(0, 0, 0, 0, 250);
                    int x = Mechanics.Roll(0, 2);
                    switch (x)
                    {
                        case 0:
                            playerInput.X = PlayerSpeed * 1.2f;
                            break;
                        case 1:
                            playerInput.X = -PlayerSpeed * 1.2f;
                            break;
                    }
                }
            }
            else if (Character.Entity.IsNoControlable)
                playerInput = Vector2.Zero;

            if (Character.Entity.IsStunned)
                playerInput = Vector2.Zero;

            if (Character.Entity.IsRooted)
                playerInput = Vector2.Zero;

            if (IsKnockedBack)
                playerInput = Vector2.Zero;

            if (IsGrounded)
                Velocity = playerInput;

            IsGrounded = false;
            for (int i = 0; i < GameMapRef.MapObjects.Length; i++)
                GameMapRef.MapObjects[i].CheckCollision(this, playerInput);

            Position += Velocity;
            Position = Vector2.Clamp(Position, new Vector2(100, -100), new Vector2(GameMapRef.MapWidth - 200, GameMapRef.MapHeight + 300));

            if (!IsGrounded)
            {
                Velocity += new Vector2(0, 0.3f);
            }
            else
            {
                IsKnockedBack = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Character.Draw(spriteBatch, boundRect);
        }

        public void ApplyStrategy(World world)
        {
            if (CombatStrategy != null)
                CombatStrategy.ApplyStrategy(this, world);
        }
       
        public void Dispose()
        {
            Character.Dispose();
            GameMapRef = null;
            NeedsDispose = true;
        }
    }
}
namespace Core
{
    public class PhotonWorldSession : WorldSession
    {
        private readonly PhotonPlayer photonPlayer;

        public override int Id => photonPlayer.ID;
        public PhotonPlayer PhotonPlayer => photonPlayer;

        public PhotonWorldSession(PhotonPlayer photonPlayer)
        {
            this.photonPlayer = photonPlayer;
        }
    }
}

namespace Core
{
    public class Pet : Guardian
    {
        public Pet(Player owner, PetType type) : base(owner, true)
        {
        }
    }
}
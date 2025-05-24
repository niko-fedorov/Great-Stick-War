namespace Game
{
    public interface IInteractable
    {
        public string ActionText { get; }
        public string Name { get; }

        //public Color TextColor { get; }

        public bool IsInteractable { get; }

        void Interact(Player player);
    }
}

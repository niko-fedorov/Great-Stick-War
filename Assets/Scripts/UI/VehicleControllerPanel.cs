using UnityEngine.UIElements;

namespace Game.UI
{
    [UxmlElement]
    public partial class VehicleControllerPanel : VisualElement
    {
        public void Initialize(Vehicle vehicle)
        {
            vehicle.SeatChanged += (player, seat) => UpdateCrew(vehicle);
        }

        private void UpdateCrew(Vehicle vehicle)
        {
            //if (_crewSlots.Count < vehicle.Crew.Count)
            //{
            //    for (int i = 0; i < vehicle.Capacity; i++)
            //    {
            //        var crewSlot = Instantiate(_crewSlotPrefab, _crewPanel.transform);
            //        crewSlot.SeatIndexText.text = i.ToString();

            //        if (vehicle.Crew.Any(crewMember => crewMember.SeatIndex == i)
            //            && vehicle.Crew.First(crewMember => crewMember.SeatIndex == i).Player.TryGet(out Player player))
            //        {
            //            crewSlot.CrewMemberNameText.text = player.Name;
            //            if (player == Player.Instance)
            //                crewSlot.CrewMemberNameText.color = UIManager.Instance.Colors[ColorType.Green];
            //        }

            //        _crewSlots.Add(crewSlot);
            //    }
            //}

            //for (int i = 0; i < _crewSlots.Count; i++)
            //{

            //}
        }
    }
}
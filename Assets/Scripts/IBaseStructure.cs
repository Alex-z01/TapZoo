using System;

public interface IBaseStructure
{
    void OnMouseDown();
    void OnTick(object sender, TickSystem.OnTickEventArgs e);
    void ToggleBaseUI(bool value);

    public Player GetPlayer();
    public Shop GetShop();

    void SubscribeToEvents();
    void UnsubscribeFromEvents();
}

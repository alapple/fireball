namespace Fireball.Weapons
{
    public interface IWeapon
    {
        void StartFire();
        void StopFire();
        float CurrentAmmo { get; }
        float MaxAmmo { get; }
        bool IsEmpty { get; }
    }
}

using MegaCrit.Sts2.Core.Commands;

namespace Laharl.LaharlCode.Extensions;

public static class AudioHelper
{
    private static readonly Random rng = new Random();
    
    private static readonly string[] attackSfx =
    {
        "res://Laharl/sfx/attack_1.mp3",
        "res://Laharl/sfx/attack_2.mp3",
        "res://Laharl/sfx/attack_3.mp3",
        "res://Laharl/sfx/attack_4.mp3",
        "res://Laharl/sfx/attack_5.mp3"
    };
    
    private static readonly string[] damagedSfx =
    {
        "res://Laharl/sfx/hurt_1.mp3",
        "res://Laharl/sfx/hurt_2.mp3",
    };
    
    private static readonly string[] victorySfx =
    {
        "res://Laharl/sfx/victory_1.mp3",
        "res://Laharl/sfx/victory_2.mp3",
        "res://Laharl/sfx/laughter.mp3"
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(attackSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(damagedSfx);
    }
    
    public static void PlayRandomVictory()
    {
        PlayRandom(victorySfx);
    }

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index]);
    }
}
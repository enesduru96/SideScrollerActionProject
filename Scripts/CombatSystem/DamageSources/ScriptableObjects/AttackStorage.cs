using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackStorage", menuName = "NocturneKeepInteractive/Combat System/AttackStorage")]
public class AttackStorage : ScriptableObject
{
    public List<AttackElement> LightAttacks = new();

    public List<AttackElement> HeavyAttacks = new();

    public List<AttackElement> AirLightAttacks = new();

    public List<AttackElement> AirHeavyAttacks = new();

    public List<AttackElement> SpecialAttacks = new();

    public List<AttackElement> AirSpecialAttacks = new();

    public List<AttackElement> ConsumableAttacks = new();


    public AttackElement GetLightAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= LightAttacks.Count)
        {
            Debug.LogError($"GetLightAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {LightAttacks.Count})");
            return null;
        }
        return LightAttacks[adjustedIndex];
    }
    public AttackElement GetNextLightAttack(int currentIndex)
    {
        int nextIndex = currentIndex % LightAttacks.Count + 1;
        return GetLightAttack(nextIndex);
    }


    public AttackElement GetHeavyAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= HeavyAttacks.Count)
        {
            Debug.LogError($"GetHeavyAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {HeavyAttacks.Count})");
            return null;
        }
        return HeavyAttacks[adjustedIndex];
    }
    public AttackElement GetNextAirLightAttack(int currentIndex)
    {
        int nextIndex = currentIndex % AirLightAttacks.Count + 1;
        return GetAirLightAttack(nextIndex);
    }


    public AttackElement GetAirLightAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= AirLightAttacks.Count)
        {
            Debug.LogError($"GetAirLightAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {AirLightAttacks.Count})");
            return null;
        }
        return AirLightAttacks[adjustedIndex];
    }
    public AttackElement GetNextHeavyAttack(int currentIndex)
    {
        int nextIndex = currentIndex % HeavyAttacks.Count + 1;
        return GetHeavyAttack(nextIndex);
    }


    public AttackElement GetAirHeavyAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= AirHeavyAttacks.Count)
        {
            Debug.LogError($"GetAirHeavyAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {AirHeavyAttacks.Count})");
            return null;
        }
        return AirHeavyAttacks[adjustedIndex];
    }
    public AttackElement GetNextAirHeavyAttack(int currentIndex)
    {
        int nextIndex = currentIndex % AirHeavyAttacks.Count + 1;
        return GetAirHeavyAttack(nextIndex);
    }


    public AttackElement GetSpecialAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= SpecialAttacks.Count)
        {
            Debug.LogError($"GetSpecialAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {SpecialAttacks.Count})");
            return null;
        }
        return SpecialAttacks[adjustedIndex];
    }
    public AttackElement GetNextSpecialAttack(int currentIndex)
    {
        int nextIndex = currentIndex % SpecialAttacks.Count + 1;
        return GetSpecialAttack(nextIndex);
    }


    public AttackElement GetAirSpecialAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= AirSpecialAttacks.Count)
        {
            Debug.LogError($"GetAirSpecialAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {AirSpecialAttacks.Count})");
            return null;
        }
        return AirSpecialAttacks[adjustedIndex];
    }
    public AttackElement GetNextAirSpecialAttack(int currentIndex)
    {
        int nextIndex = currentIndex % AirSpecialAttacks.Count + 1;
        return GetAirSpecialAttack(nextIndex);
    }


    public AttackElement GetConsumableAttack(int index)
    {
        int adjustedIndex = index - 1;
        if (adjustedIndex < 0 || adjustedIndex >= ConsumableAttacks.Count)
        {
            Debug.LogError($"GetConsumableAttack({index}) için geçersiz index! " +
                           $"(1 tabanlý bekleniyor, liste uzunluðu: {ConsumableAttacks.Count})");
            return null;
        }
        return ConsumableAttacks[adjustedIndex];
    }

    public AttackElement GetNextConsumableAttack(int currentIndex)
    {
        int nextIndex = currentIndex % ConsumableAttacks.Count + 1;
        return GetConsumableAttack(nextIndex);
    }







    public AttackElement GetAttack(AttackID id)
    {
        // LightAttack_1..LightAttack_6
        if (id >= AttackID.LightAttack_1 && id <= AttackID.LightAttack_6)
        {
            // Örnek: LightAttack_1 -> index = 1, LightAttack_2 -> index = 2, vs.
            // Burada "AttackID.LightAttack_1" enum deðerini baz alýp aradan çýkartýyoruz.
            int index = (int)id - (int)AttackID.LightAttack_1 + 1;
            return GetLightAttack(index); // Sizin mevcut metodunuz: "LightAttacks" listesinden alýr
        }

        // AirLightAttack_1..AirLightAttack_3
        if (id >= AttackID.AirLightAttack_1 && id <= AttackID.AirLightAttack_3)
        {
            int index = (int)id - (int)AttackID.AirLightAttack_1 + 1;
            return GetAirLightAttack(index);
        }

        // HeavyAttack_1..HeavyAttack_6
        if (id >= AttackID.HeavyAttack_1 && id <= AttackID.HeavyAttack_6)
        {
            int index = (int)id - (int)AttackID.HeavyAttack_1 + 1;
            return GetHeavyAttack(index);
        }

        // AirHeavyAttack_1..AirHeavyAttack_3
        if (id >= AttackID.AirHeavyAttack_1 && id <= AttackID.AirHeavyAttack_3)
        {
            int index = (int)id - (int)AttackID.AirHeavyAttack_1 + 1;
            return GetAirHeavyAttack(index);
        }

        // SpecialAttack_1..SpecialAttack_8 (örneðin 8'e kadar olduðunu varsaydýk)
        if (id >= AttackID.SpecialAttack_1 && id <= AttackID.SpecialAttack_8)
        {
            int index = (int)id - (int)AttackID.SpecialAttack_1 + 1;
            return GetSpecialAttack(index);
        }

        // AirSpecialAttack_1..AirSpecialAttack_3
        if (id >= AttackID.AirSpecialAttack_1 && id <= AttackID.AirSpecialAttack_3)
        {
            int index = (int)id - (int)AttackID.AirSpecialAttack_1 + 1;
            return GetAirSpecialAttack(index);
        }

        // ConsumableAttack_1..3
        if (id >= AttackID.ConsumableAttack_1 && id <= AttackID.ConsumableAttack_3)
        {
            int index = (int)id - (int)AttackID.ConsumableAttack_1 + 1;
            return GetConsumableAttack(index);
        }

        // Eþleþme yoksa:
        Debug.LogWarning("Attack ID Not Found");
        return null;
    }





    public List<AttackElement> GetAllAttacks()
    {
        List<AttackElement> allAttacks = new List<AttackElement>();

        allAttacks.AddRange(LightAttacks);
        allAttacks.AddRange(HeavyAttacks);
        allAttacks.AddRange(AirLightAttacks);
        allAttacks.AddRange(AirHeavyAttacks);
        allAttacks.AddRange(SpecialAttacks);
        allAttacks.AddRange(AirSpecialAttacks);
        allAttacks.AddRange(ConsumableAttacks);

        return allAttacks;
    }

}
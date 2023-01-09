using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BreedingOutcome
{
    public string outcome;
    public int chance;
    private float weight;

    public void SetWeight(float weight) { this.weight = weight; }
    public float GetWeight() { return this.weight; }
}

[Serializable]
public class BreedingCombination
{
    public string first, second;
    public List<BreedingOutcome> outcomes = new List<BreedingOutcome>();
}

public class AnimalBreeding : MonoBehaviour
{
    public List<BreedingCombination> combinations = new List<BreedingCombination>();

    private void Start()
    {
        CalculateWeights();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SimulateRNG();
        }
    }

    void SimulateRNG()
    {
        int chickens = 0;
        int cows = 0;
        int chows = 0;
        int piggies = 0;
        BreedingOutcome outcome = new BreedingOutcome();
        BreedingCombination combo = GetBreedingCombination("Chicken", "Cow");

        float iterations = 50000;

        for (int i = 0; i < iterations; i++)
        {
            outcome = Breed(combo);
            if(outcome.outcome == "Chicken") { chickens++; }
            if(outcome.outcome == "Cow") { cows++; }
            if(outcome.outcome == "Chow") { chows++; } 
            if(outcome.outcome == "Piggy") { piggies++; }
        }

        Debug.Log(
            $"{iterations} iterations\n" +
            $"Chickens: {chickens}, {chickens / iterations * 100}%\n" +
            $"Cows: {cows}, {cows / iterations * 100}%\n" +
            $"Chows: {chows}, {chows / iterations * 100}%\n" +
            $"Piggies: {piggies}, {piggies / iterations * 100}%\n" 
            );
    }

    void CalculateWeights()
    {
        foreach(BreedingCombination comb in combinations )
        {
            foreach(BreedingOutcome outcome in comb.outcomes)
            {
                float weight = (outcome.chance / 100f) * (comb.outcomes.Count * 100f);
                outcome.SetWeight(weight);
            }
        }
    }

    BreedingCombination GetBreedingCombination(string a1, string a2)
    {
        BreedingCombination combo = null;

        foreach (BreedingCombination combination in combinations)
        {
            if (combination.first == a1 && combination.second == a2)
            {
                combo = combination;
            }
        }

        return combo;
    }

    BreedingOutcome Breed(BreedingCombination combo)
    {
        BreedingOutcome _outcome = new BreedingOutcome();

        float totalWeight = 0;

        foreach (BreedingOutcome outcome in combo.outcomes)
        {
            totalWeight += outcome.GetWeight();
        }

        float rand = UnityEngine.Random.Range(1, totalWeight + 1);
        float runningTotal = 0;

        foreach(BreedingOutcome outcome in combo.outcomes)
        {
            runningTotal += outcome.GetWeight();
            if(rand <= runningTotal)
            {
                _outcome = outcome;
                break;
            }
        }

        return _outcome;
    }
}



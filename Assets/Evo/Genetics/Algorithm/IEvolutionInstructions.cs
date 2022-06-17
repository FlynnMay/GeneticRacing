namespace Evo
{
    public interface IEvolutionInstructions
    {
        // Use to create a custom fitness calculation
        public float EvolutionFitnessFunction(Genome genome);

        // Use to generate custom random values for the genomes
        public object GetEvolutionRandomValue();
    }
}

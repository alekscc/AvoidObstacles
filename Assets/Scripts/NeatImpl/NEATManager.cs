//using NeatImpl;
//using Statistics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeatImpl
//{
//    class NEATManager {

//        public int Generation { get; private set; }

//        public Genome[] Population { get; private set; }

//        public int PopulationSize { get; private set; }

//        public Genome Solution { get; private set; }

//        private List<Species> species;

//        private Random random;

//        private float survivalThreshold = .9f;
//        private float elitismThreshold = .05f;

//        private float speciesMaxDistance = 5f;
//        private float coef1 = 1f;
//        private float coef2 = 1f;
//        private float coef3 = .4f;


//        private float mutateLinkChance = 0.02f;
//        private float mutateNodeChance = 0.01f;
//        private float mutateWeightChance = 0.02f;
//        private float mutateWeightShiftChance = 0.02f;
//        private float mutateToggleEnableChance = 0.02f;

//        private int elitismMinSize, elitismNumber;

//        private GenePool pool;

//        private TrainingConfigurationBase configuration;
//        // statistics

//        public StatisticsCollector stats { get; private set; }



//        public NEATManager(int nInputs, int nOutputs, int number, TrainingConfigurationBase configuration) {
//            this.configuration = configuration;

//            survivalThreshold = 1 - configuration.populationRecudctionLevel;
//            //elitismThreshold = configuration.elitism;
//            speciesMaxDistance = configuration.speciesDistance;

//            mutateLinkChance = configuration.mutateConnectionChance;
//            mutateNodeChance = configuration.mutateNodeChance;
//            mutateWeightChance = configuration.mutateWeightChance;
//            mutateWeightShiftChance = configuration.mutateWeightShiftChance;
//            mutateToggleEnableChance = configuration.mutateToggleEnable;

//            elitismNumber = configuration.elitismNumber;
//            elitismMinSize = configuration.elitismMinSize;

//            coef1 = configuration.coef1;
//            coef2 = configuration.coef2;
//            coef3 = configuration.coef3;

//            //GenePool.Instance().UnityDebugInfo();



//            random = new Random();

//            pool = new GenePool(random);

//            stats = new StatisticsCollector();

//            Generation = 1;

//            species = new List<Species>();

//            Population = new Genome[number];

//            for (int i = 0; i < Population.Length; i++) {
//                Population[i] = new Genome(nInputs,
//                                            nOutputs,
//                                            this.configuration.startPopulationComplexityLevel,
//                                            random, pool);
//            }

//            //int populationHalfSize = (int)(Population.Length * .5);
//            //for (int i = 0; i < populationHalfSize; i++)
//            //{
//            //        Population[i] = new Genome(nInputs,
//            //                                    nOutputs,
//            //                                    this.configuration.startPopulationComplexityLevel,
//            //                                    random,pool);

//            //}
//            //for(int i = populationHalfSize; i < Population.Length; i++)
//            //{
//            //    int siblingIndex = i-populationHalfSize;
//            //    Population[i] = Population[siblingIndex].Clone() as Genome;

//            //}

//            PopulationSize = number;

//            GenerateSpecies();
//        }
//        //~NEATManager()
//        //{
//        //    stats.SaveToFile("backup_"+STATS_FILE_NAME);
//        //}
//        public void SaveStats(string directory, string filename) {

//            Utils.Utilities.MakeDirectory(directory);

//            stats.SaveToFile(directory + "/" + filename);
//        }

//        public void SaveSolution(string directory, string filename) {
//            Utils.Utilities.MakeDirectory(directory);

//            GenomeData data = new GenomeData(Solution);

//            Utils.Utilities.XmlSave(directory + "/" + filename, data);
//        }
//        public void SaveConfiguration(string directory, string filename) {
//            Utils.Utilities.MakeDirectory(directory);

//            Utils.Utilities.XmlSave(directory + "/" + filename, configuration);
//        }
//        public void Evolve() {
//            SelectRecord();

//            Generation++;

//            UnityEngine.Debug.Log("Generating species");

//            GenerateSpecies();

//            //UnityEngine.Debug.Log($"Population size:{Population.Count()}");

//            UnityEngine.Debug.Log("Reducing speices populations");

//            ReduceSpecies(1 - survivalThreshold);

//            // UnityEngine.Debug.Log($"Population size:{Population.Count()}");

//            UnityEngine.Debug.Log("Killing extinct speceis");

//            KillExtinctSpecies();

//            //  UnityEngine.Debug.Log($"Population size:{Population.Count()}");

//            UnityEngine.Debug.Log("Reproducing new populations");

//            Reproduce();

//            //   UnityEngine.Debug.Log($"Population size:{Population.Count()}");

//            UnityEngine.Debug.Log("Mutations");

//            Mutate();

//            //    UnityEngine.Debug.Log($"Population size:{Population.Count()}");



//        }
//        private void ReduceSpecies(float perc) {
//            foreach (Species spec in species) {
//                spec.Kill(perc);
//            }
//        }
//        public void SelectRecord() {
//            Genome currentSolution = Population.OrderByDescending(x => x.Fitness).FirstOrDefault().Clone() as Genome;

//            //UnityEngine.Debug.Log("Show all genomes fitness and spices");

//            //int _n = 0;
//            //foreach(var g in Population.OrderByDescending(x => x.Fitness))
//            //{
//            //   // UnityEngine.Debug.Log($"g[{_n++}] fitness:{g.Fitness} spices:{g.species}");
//            //}

//            if (Solution == null) {
//                Solution = currentSolution;
//            }
//            else {
//                Solution = (Solution.Fitness > currentSolution.Fitness) ? Solution : currentSolution;
//                UnityEngine.Debug.Log("New Solution: " + Solution.Fitness);
//            }

//            // general stats

//            float fitnessMin = Population.Min(x => x.Fitness);
//            float fitnessAvg = Population.Sum(x => x.Fitness) / (float)Population.Length;

//            stats.RecordFitness(Generation, Solution.Fitness, fitnessMin, fitnessAvg);
//            stats.RecordSpecies(Generation, species.Count);


//        }
//        private void Mutate() {

//            //foreach(var g in Population.OrderByDescending(x=>x.Fitness).Skip((int)(Population.Length*ELITISM_THRESHOLD)))
//            //{
//            //    g.Mutate(random);
//            //}
//            int nSkippedMutation = 0;
//            foreach (var g in Population.Where(x => !x.isElite)) {

//                g.Mutate(mutateLinkChance, mutateNodeChance, mutateWeightChance, mutateWeightShiftChance, mutateToggleEnableChance, random);

//                g.Evaluate(-555f);
//            }

//            UnityEngine.Debug.Log($"Skipped mutation:{nSkippedMutation}");

//            //int _toSkip = (int)(Population.Length * ELITISM_CHANCE);
//            //foreach(Genome g in Population.OrderByDescending(x=>x.Fitness).Skip(_toSkip))
//            //{
//            //    g.Mutate(random);
//            //}
//        }
//        private Genome[] SelectEliteGenomes(int n, int minSpecieSize) {
//            List<Genome> eliteGenomes = new List<Genome>();

//            if (n > 0) {
//                foreach (var spec in species.Where(x => x.Population.Count > minSpecieSize)) {
//                    eliteGenomes.Add(spec.Population.OrderByDescending(x => x.Fitness).FirstOrDefault().Clone() as Genome);
//                }
//            }

//            return eliteGenomes.ToArray();
//        }
//        private Species GetSpeciesByTournament(IEnumerable<Species> species, float percGroupSize) {

//            int nGroupSize = (int)(percGroupSize * species.Count());


//            if (nGroupSize <= 1) {
//                return species.OrderBy(x => random.Next()).FirstOrDefault();
//            }


//            return species.OrderBy(x => random.Next()).Take(nGroupSize).OrderByDescending(x => x.Score).FirstOrDefault();

//        }
//        private float Clamp01(float min, float max, float value) {

//            return (value < min) ? min : (value > max) ? max : value;

//        }

//        private void Reproduce() {
//            var eliteGenomes = species.Where(x => x.Population.Count > elitismMinSize).Select(x => new { Genome = x.Population.OrderByDescending(y => y.Fitness).FirstOrDefault(), Spec = x }).ToList();

//            int speciesPopulationSize = species.Sum(x=>x.Population.Count);

//            UnityEngine.Debug.Log($"pre reproduce species size: {speciesPopulationSize}/{Population.Length}");

//            int leftPopulationSlots = PopulationSize - eliteGenomes.Count();

//            float totalSpeciesScore = species.Sum(x => x.Score);

//            Dictionary<Species, int> offspringPerSpecies = new Dictionary<Species, int>();


//            List<Species> speciesToRemove = new List<Species>();

//            for (int i = species.Count-1; i >= 0; i--) {

//                Species spec = species[i];

//                //if(spec.Score <= 0f) {
//                //    offspringPerSpecies.Add(spec, 0);
//                //}

//                float percentage = Clamp01(0f, 1f, spec.Score / totalSpeciesScore);

//                int nOffsprings = (int)Math.Floor(percentage * leftPopulationSlots);

//                if (nOffsprings >0) {
//                    offspringPerSpecies.Add(spec, nOffsprings);
//                }
//                else {
//                    //     speciesToRemove.Add(spec);

//                    offspringPerSpecies.Add(spec, 1);

//                }


//            }

//            //foreach(var item in speciesToRemove) {
//            //    item.Extinct();
//            //    species.Remove(item);
//            //}


//            leftPopulationSlots -= offspringPerSpecies.Sum(x => x.Value);

//            while (leftPopulationSlots > 0) {

//                offspringPerSpecies[species.OrderBy(x => random.Next()).FirstOrDefault()]++;

//                leftPopulationSlots--;
//            }

//            while(leftPopulationSlots < 0) {

//                offspringPerSpecies[offspringPerSpecies.OrderByDescending(x => x.Value).FirstOrDefault().Key]--;
//                leftPopulationSlots++;

//            }


//            List<Genome> newPopulation = new List<Genome>();

//            foreach (var item in offspringPerSpecies) {

//                Species spec = item.Key;
//                int nLeft = item.Value;

//                Genome[] offsprings = new Genome[nLeft];

//                while (nLeft > 0) {

//                    offsprings[--nLeft] = spec.Breed(random);

//                }

//                spec.Extinct();

//                foreach (var genome in offsprings)
//                    spec.ForceAdd(genome);

//                //spec.SelectNewRepresentative(random);

//                newPopulation.AddRange(offsprings);
//            }

//            speciesPopulationSize = species.Sum(x => x.Population.Count);

//            //if (speciesPopulationSize != Population.Length) {
//            //    UnityEngine.Debug.LogError($"Population oversize: {speciesPopulationSize}/{Population.Length}");
//            //}

//            eliteGenomes.ForEach(x => x.Genome.isElite = true);
//            eliteGenomes.ForEach(x => x.Spec.ForceAdd(x.Genome));

            

//            newPopulation.AddRange(eliteGenomes.Select(x => x.Genome));

//            speciesPopulationSize = species.Sum(x => x.Population.Count);

//            if (speciesPopulationSize != Population.Length) {
//                UnityEngine.Debug.LogError($"Population oversize: {speciesPopulationSize}/{Population.Length}");
//            }

//            Population = newPopulation.ToArray();

//            //foreach (var item in eliteGenomes) {

//            //    item.Genome.isElite = true;

//            //    if (!item.Spec.color.SequenceEqual(item.Genome.parentColor)) {
//            //        UnityEngine.Debug.Log("kolorki sie nie zgadzaja");
//            //    }

//            //    item.Spec.ForceAdd(item.Genome);
//            //}






//            //if (Generation > 1) {

//            //    foreach (var item in Population) {
//            //        if (!item.species.color.SequenceEqual(item.parentColor)) {
//            //            UnityEngine.Debug.Log("kolorki sie nie zgadzaja");
//            //        }
//            //    } 
//            //}

//            //float totalScore = species.Sum(x => x.Score);

//            //// separeting elite genomes

//            //int populationSize = Population.Length;


//            //Population = species.SelectMany(x => x.Population).ToArray();


//            ////int nEliteGenomes = (int)(populationSize * elitismThreshold);

//            //Genome[] eliteGenomes = SelectEliteGenomes(elitismNumber,elitismMinSize);

//            ////eliteGenomes = Population.OrderByDescending(x => x.Fitness).Take(nEliteGenomes).Select(x=>x.Clone() as Genome).ToArray();


//            //// calculating number of offsprings for every species

//            //int[] nOffspringsPerSpecies = new int[species.Count];
//            //int nPopulationLeft = populationSize - eliteGenomes.Length;

//            ////Dictionary<Species, int> speciesDistributionMap = new Dictionary<Species, int>();

//            ////foreach (Species spec in species)
//            ////    speciesDistributionMap.Add(spec, 0);

//            ////int iLeftToAssign = nPopulationLeft;
//            ////while (iLeftToAssign-- > 0) {

//            ////    speciesDistributionMap[GetSpeciesByTournament(species.Select(x => x), .5f)]++;
//            ////}

//            ////nOffspringsPerSpecies = speciesDistributionMap.Select(x => x.Value).ToArray();

//            //for (int i = 0; i < species.Count; i++) {
//            //    if (species[i].Score <= 0f) {
//            //        nOffspringsPerSpecies[i] = 0;
//            //        continue;
//            //    }
//            //    float perc = species[i].Score / totalScore;

//            //    nOffspringsPerSpecies[i] = (int)(nPopulationLeft * perc);
//            //    UnityEngine.Debug.LogWarning($"ID:{i} score:{species[i].Score} {perc}% n={nOffspringsPerSpecies[i]}");
//            //}

//            //int nNotAssignedOffsprings = nPopulationLeft - nOffspringsPerSpecies.Sum();

//            //while (nNotAssignedOffsprings > 0) {
//            //    nOffspringsPerSpecies[random.Next(nOffspringsPerSpecies.Length)]++;

//            //    nNotAssignedOffsprings--;
//            //}

//            //// breeding offsprings for every species proportional to previous calculations


//            //List<Genome> newPopulation = new List<Genome>();
//            ////UnityEngine.Color genColor = new UnityEngine.Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
//            //for (int i = 0; i < species.Count; i++)
//            //{
//            //    Genome[] newSpeciesPopulation = new Genome[1];

//            //    try {
//            //        newSpeciesPopulation = new Genome[nOffspringsPerSpecies[i]];
//            //    }
//            //    catch (OverflowException e) {

//            //        UnityEngine.Debug.Log("overflow");
//            //    }

//            //    while (nOffspringsPerSpecies[i] > 0)
//            //    {
//            //        Genome offspring = species[i].Breed(random);
//            //        newSpeciesPopulation[--nOffspringsPerSpecies[i]] = offspring;
//            //        //offspring.color = genColor;
//            //    }

//            //    species[i].Population.Clear();

//            //    foreach(var item in newSpeciesPopulation)
//            //    {
//            //        species[i].ForceAdd(item);
//            //    }

//            //    species[i].Population = newSpeciesPopulation.ToList();

//            //    newPopulation.AddRange(newSpeciesPopulation);
//            //}


//            //// adding elite genomes to main population list and to species
//            ////int eliteGenomeAssigned = 0;

//            //foreach (var g in eliteGenomes)
//            //{

//            //    g.isElite = true;
//            //    g.Evaluate(-100f);

//            //    //UnityEngine.Debug.LogWarning($"ELITE FIT:{g.Fitness}");

//            //    if (g.species.Population.Find(x => x.CompareTo(g) == 0)!=null)
//            //    {
//            //        UnityEngine.Debug.Log("Elite genome already in species!");
//            //    }
//            //    else
//            //    {
//            //        UnityEngine.Debug.Log("Adding elite gneome to species");

//            //        g.species.ForceAdd(g);

//            //    }
//            //}

//            ////if (eliteGenomeAssigned>0)
//            ////{
//            ////    UnityEngine.Debug.Log($"Elite genome  assigned:{eliteGenomeAssigned}"); 
//            ////}
//            ////else
//            ////{
//            ////    UnityEngine.Debug.Log("ERROR ELITE GENOEMS ARE NOT ASSIGEND");
//            ////}

//            //newPopulation.AddRange(eliteGenomes);

//            //// applying new populaiton

//            //Population = newPopulation.ToArray();

//            //// debug, final check

//            //UnityEngine.Debug.Log($"Number of new population:{newPopulation.Count}/{populationSize}");

//            //int nInvalidSpeciesGenome = 0;
//            //foreach(var item in Population)
//            //{
//            //    if (item.species != null)
//            //    {
//            //        if (item.species.Population.Find(x => x.identifier == item.identifier)!=null)
//            //            continue;
//            //    }

//            //    nInvalidSpeciesGenome++;
//            //}

//            //if(Population.Length < populationSize)
//            //{
//            //    UnityEngine.Debug.Log("problem populationSize < Population.Length");
//            //}

//            //UnityEngine.Debug.Log($"Reproduction final check; invalide species genomes:{nInvalidSpeciesGenome}");
//        }

//        private void KillExtinctSpecies() {
//            List<Species> speciesToRemove = new List<Species>();

//            int speciesMaxPopulation = species.Max(x => x.Population.Count);


//            if (speciesMaxPopulation < 2)
//                return;

//            foreach (var spec in species) {

//                if (spec.Population.Count < 2) {
//                    spec.Extinct();
//                    speciesToRemove.Add(spec);
//                }
//            }

//            speciesToRemove.ForEach(x => species.Remove(x));


//        }

//        //private void KillExtinctSpecies() {
//        //    UnityEngine.Debug.Log($"Number of species before extinct:{species.Count}");

//        //    List<Species> speciesToRemove = new List<Species>();


//        //    int specieMaxPopulation = species.Max(x => x.Population.Count());

//        //    UnityEngine.Debug.Log($"species max population:{specieMaxPopulation}");

//        //    if (specieMaxPopulation < 2)
//        //        return;

//        //    //float maxFit = species.Max(x => x.Score);

//        //    for (int i = 0; i < species.Count; i++) {

//        //        // UnityEngine.Debug.Log($"[{i}] / {species.Count} species population:{species[i].Population.Count}");

//        //        //if (species[i].Score == maxFit)
//        //        //    continue;

//        //        if (species[i].Population.Count < 2) {
//        //            species[i].Extinct();

//        //            speciesToRemove.Add(species[i]);

//        //            //if (species.Remove(species[i])) {
//        //            //    UnityEngine.Debug.Log($"Removed species:{i}");
//        //            //}
//        //            //else {
//        //            //   // UnityEngine.Debug.Log($"NOT Removed species:{i}");
//        //            //}
//        //        }

//        //    }

//        //    foreach (Species spec in speciesToRemove)
//        //        species.Remove(spec);

//        //    UnityEngine.Debug.Log($"Number of species after extinct:{species.Count}");
//        //}
//        private void GenerateSpecies() {
//            //int _nAttemp = 1;
//            //bool _isAssignedCorrectly = false;

//            //species.ForEach(x => x.Reset(random));

//            int totalSpeciesPopulation = species.Sum(x => x.Population.Count);
//            //if (totalSpeciesPopulation != Population.Length) {
//            //    UnityEngine.Debug.LogError($" error species populatation size :{totalSpeciesPopulation}/{Population.Length}");
//            //}
//            //else UnityEngine.Debug.Log("no species error");

//            foreach (var item in species)
//                item.Reset();

//            foreach (var genome in Population.Where(x => x.species == null)) {

//                bool isAssigned = false;

//                foreach (var spec in species) {


//                    if (spec.Add(genome, speciesMaxDistance, coef1, coef2, coef3)) {
//                        isAssigned = true;
//                        break;
//                    }

//                }

//                if (!isAssigned) {
//                    Species newSpecies = new Species(genome);
//                    species.Add(newSpecies);
//                }

//            }

//            species.ForEach(x => x.Evaluate());

//            totalSpeciesPopulation = species.Sum(x => x.Population.Count);

//            if (totalSpeciesPopulation != Population.Length) {
//                UnityEngine.Debug.LogError($" error species populatation size :{totalSpeciesPopulation}/{Population.Length}");
//            }


//            //    //do
//            //    //{
//            //    int _nAssignded = 0;
//            //    //foreach (Genome item in Population)
//            //    //{
//            //    //    if (item.species != null)
//            //    //    {
//            //    //        if(item.species.Population.Find(x => x.Equals(item)) != null){
//            //    //            _nAssignded++;
//            //    //        }
//            //    //        else
//            //    //        {
//            //    //            UnityEngine.Debug.Log($"genome wrong assigned! {item.identifier}");


//            //    //        }
//            //    //    }
//            //    //}
//            //    int nIndividualsBeftAssignationInSpecies = 0;

//            //    foreach (var item in species) {
//            //        nIndividualsBeftAssignationInSpecies += item.Population.Count;
//            //    }

//            //    UnityEngine.Debug.LogWarning($"Number of individuals in species before assignation:{nIndividualsBeftAssignationInSpecies}");


//            //    for (int i = 0; i < Population.Length; i++) {
//            //        if (Population[i].species != null) {
//            //            if (Population[i].species.Population.Find(x => x.CompareTo(Population[i]) == 0) != null) {
//            //                _nAssignded++;
//            //            }
//            //            else {
//            //                UnityEngine.Debug.LogError($"genome wrong assigned! {Population[i].identifier}");
//            //            }
//            //        }
//            //    }


//            //    if (_nAssignded != Population.Length && _nAssignded > 0) {
//            //        UnityEngine.Debug.Log($"population assigend:{_nAssignded}/{Population.Length}");
//            //    }

//            //    int _nResetedSpecies = 0;
//            //    List<Species> speciesToRemove = new List<Species>();
//            //    foreach (Species spec in species) {
//            //        if (spec.Population.Count > 0) {
//            //            spec.Reset(random);
//            //        }
//            //        else {
//            //            speciesToRemove.Add(spec);
//            //        }

//            //        _nResetedSpecies++;
//            //    }

//            //    speciesToRemove.ForEach(x => { x.Extinct(); species.Remove(x); });

//            //    UnityEngine.Debug.Log($"Reseted species:{_nResetedSpecies}");

//            //    int _nAssignedToSpecies = 0,
//            //        _nCreatedNewSpecies = 0,
//            //        _nSkipped = 0;
//            //    foreach (Genome g in Population) {
//            //        if (g.species != null) {
//            //            _nSkipped++;
//            //            continue;
//            //        }
//            //        bool _isAssigned = false;

//            //        foreach (Species spec in species) {
//            //            _isAssigned = spec.Add(g, speciesMaxDistance, coef1, coef2, coef3 /** _nAttemp*/);
//            //            if (_isAssigned) {
//            //                _nAssignedToSpecies++;

//            //                break;
//            //            }
//            //        }

//            //        if (!_isAssigned) {
//            //            _nCreatedNewSpecies++;
//            //            species.Add(new Species(g));
//            //        }
//            //    }

//            //    UnityEngine.Debug.Log($"Species assigned:{_nAssignedToSpecies}; created:{_nCreatedNewSpecies} skipped:{_nSkipped}");



//            //    UnityEngine.Debug.Log($"Number of species:{species.Count} max:{species.Max(x => x.Population.Count)}");


//            //    //_isAssignedCorrectly = species.Max(x => x.Population.Count) >= 2;

//            //    //if (!_isAssignedCorrectly)
//            //    //{
//            //    //    _nAttemp++;
//            //    //    foreach(Species spec in species)
//            //    //    {
//            //    //        spec.Extinct();
//            //    //    }
//            //    //    species.Clear();

//            //    //    UnityEngine.Debug.LogWarning("Reseted all spices");
//            //    //}

//            //    //} while (!_isAssignedCorrectly);

//            //    //UnityEngine.Debug.Log("species assignation attemp:"+_nAttemp);

//            //    int nTotalIndividualsInSpecies = 0;

//            //    foreach (Species spec in species) {
//            //        spec.Evaluate();

//            //        UnityEngine.Debug.Log($"Species score:{spec.Score} number:{spec.Population.Count}");

//            //        nTotalIndividualsInSpecies += spec.Population.Count;
//            //    }


//            //    if (nTotalIndividualsInSpecies > Population.Length) {
//            //        UnityEngine.Debug.LogError($"Invalid number of total individuals in species! {nTotalIndividualsInSpecies} of {Population.Length} in Population");
//            //    }


//            //    UnityEngine.Debug.LogWarning($"Total number of species:{species.Count}");

//            //}

//        }
//    }

//}

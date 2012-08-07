using System;
using System.Collections.Generic;
using System.Text;

namespace TaskOptimizer.Interfaces
{
    public abstract class Population<IndividualType> where IndividualType : Individual
    {
        public virtual void optimize()
        {
            m_nbIterationsWithoutImprovements++;
            m_curIteration++;

            computeCrossovers();
            computeMutations();
            computeCataclysms();

            m_bestIndividual.optimize();
        }

        protected virtual void computeCrossovers()
        {
            if (m_populationSize == 1)
            {
                return;
            }

            IndividualType parent;
            int nbChildren = 0;
            int maxChildren = computeMaxChildren();

            while (nbChildren < maxChildren && (parent = selectHealthyIndividual()) != null)
            {
                if (crossover(parent))
                {
                    nbChildren++;
                }
            }
        }

        protected virtual bool crossover(IndividualType parent)
        {
            IndividualType p1, p2, child;
            selectCrossoverIndividuals(out p1, out p2, out child);

            child.crossover(p1, p2);

            if (child.Fitness < m_bestFitness)
            {
                onNewBestIndividual(child);
            }

            return true;
        }

        protected bool canWeakDieByCrossover(IndividualType weak, IndividualType parent)
        {
            return weak.Id != parent.Id;
        }

        protected virtual void onNewBestIndividual(IndividualType individual)
        {
            // really a new best?
            if (individual.Fitness < m_bestFitness)
            {
                m_mutationRate = m_initialMutationRate;
                m_cataclysmCountdown = m_initialCataclysmCountdown;
                m_nbIterationsWithoutImprovements = 0;
            }

            m_bestIndividual = individual;
            m_bestFitness = individual.Fitness;
        }

        protected virtual void computeMutations()
        {
            if (m_populationSize == 1)
            {
                return;
            }

            int maxMutations = computeMaxMutations();

            for (int t = 0; t < maxMutations; t++)
            {
                if (m_rand.Next(m_mutationRate) == 0)
                {
                    selectWeakIndividual().mutate();
                }
            }

            m_mutationRate--;

            if (m_mutationRate < 1)
            {
                m_mutationRate = 1;
            }

        }



        protected virtual int computeMaxChildren()
        {
            return (int)(m_populationSize * 0.03 + 2);
        }

        protected virtual int computeMaxMutations()
        {
            return (int)(m_populationSize * 0.03 + 2);
        }


        protected virtual void computeCataclysms()
        {
            if (isCataclysmTime())
            {
                // boom! regenerate new distributions...
                regeneratePopulation();

                m_cataclysmCountdown = m_initialCataclysmCountdown;
            }

            m_cataclysmCountdown--;
        }


        protected bool isCataclysmTime()
        {
            if (m_populationSize == 1)
            {
                return false;
            }

            return m_cataclysmCountdown <= 0;
        }

        protected abstract void regeneratePopulation();



        protected virtual IndividualType selectWeakIndividual()
        {
            int maxFitness = Int32.MinValue;
            IndividualType weakestIndividual = default(IndividualType);

            foreach (IndividualType individual in m_individuals)
            {
                if (individual != null && individual.Fitness > maxFitness && individual.Id != m_bestIndividual.Id)
                {
                    weakestIndividual = individual;
                    maxFitness = individual.Fitness;
                }
            }

            return weakestIndividual;
        }

        protected virtual void selectCrossoverIndividuals(out IndividualType p1, out IndividualType p2, out IndividualType child)
        {
            p1 = m_individuals[m_rand.Next() % m_populationSize];
            p2 = m_individuals[m_rand.Next() % m_populationSize];
            child = selectWeakIndividual();

            if (p1.Id == p2.Id || p1.Id == child.Id || p2.Id == child.Id)
            {
                selectCrossoverIndividuals(out p1, out p2, out child);
            }
        }


        protected virtual IndividualType selectHealthyIndividual()
        {
            int nbTries = 0;
            int curIndex = m_rand.Next(m_populationSize);
            for (int t = 0; t < m_populationSize; t++)
            {
                IndividualType individual = m_individuals[curIndex % m_populationSize];

                if (individual != null)
                {
                    if (individual.Id != m_bestIndividual.Id)
                    {
                        int odds = (individual.Fitness * 3 / (m_bestFitness)) + 1;

                        int n = m_rand.Next(odds);

                        if (n == 0)
                        {
                            return individual;
                        }
                    }
                    nbTries++;
                    curIndex++;
                }
            }

            return default(IndividualType);
        }

        protected IndividualType[] m_individuals;
        protected IndividualType m_bestIndividual = default(IndividualType);
        protected int m_bestFitness = Int32.MaxValue;

        protected int m_populationSize = 10;
        protected int m_cataclysmCountdown, m_initialCataclysmCountdown = 1000;
        protected int m_mutationRate, m_initialMutationRate = 70;
        protected int m_curIteration = 0, m_nbIterationsWithoutImprovements = 0, m_maxIterationsWithoutImprovements;
        protected Random m_rand = new Random();

    }

}

using System;
using System.Collections.Generic;
using System.Drawing;
using TaskOptimizer.API;

namespace TaskOptimizer.Model
{
    internal class TaskGenerator
    {
        private readonly Random m_rand;

        public TaskGenerator(int randomSeed)
        {
            m_rand = new Random(randomSeed);
        }

        public List<Task> generateTasks(int nbTasks, bool randomTaskSizes, int startX, int startY)
        {
            var tasks = new List<Task>(nbTasks);

            for (int t = 0; t < nbTasks; t++)
            {
                tasks.Add(generateTask(t, nbTasks, randomTaskSizes));
            }

            foreach (Task task in tasks)
            {
                task.setDefaultDistance(computeDistance(startX, startY, task));

                foreach (Task to in tasks)
                {
                    task.setDistance(to, computeDistance(task.X, task.Y, to));
                }
            }

            return tasks;
        }

        private int computeDistance(double x, double y, Task task)
        {
            return Precomp.getDistance(new Coordinate(x, y), new Coordinate(task.lat, task.lon));
        }

        private Task generateTask(int id, int nbTasks, bool randomTaskSizes)
        {
            var task = new Task(id, nbTasks);
            if (randomTaskSizes)
            {
                task.Effort = m_rand.Next(50) + 10;
                if (m_rand.Next(nbTasks) == 0)
                {
                    task.Effort = 150;
                }
            }
            else
            {
                task.Effort = 20;
            }
            task.X = m_rand.Next(1000);
            task.Y = m_rand.Next(600);


            return task;
        }

        public List<Robot> generateRobots(int nbRobots)
        {
            var robots = new List<Robot>(nbRobots);

            for (int t = 0; t < nbRobots; t++)
            {
                robots.Add(generateRobot());
            }
            /*
            // big
            Robot robot = new Robot();
            robot.Color = Color.Red;
            robot.DistanceCost = 200;
            robot.WorkCost = 200;
            robot.DistanceTime = 10;
            robot.WorkTime = 1;
            robots.Add(robot);

            // fast
            robot = new Robot();
            robot.Color = Color.Green;
            robot.DistanceCost = 40;
            robot.WorkCost = 10;
            robot.DistanceTime = 1;
            robot.WorkTime = 300;
            robots.Add(robot);

            // medium
            robot = new Robot();
            robot.Color = Color.Blue;
            robot.DistanceCost = 50;
            robot.WorkCost = 50;
            robot.DistanceTime = 5;
            robot.WorkTime = 30;
            robots.Add(robot);

            // medium2
            robot = new Robot();
            robot.Color = Color.DarkBlue;
            robot.DistanceCost = 50;
            robot.WorkCost = 50;
            robot.DistanceTime = 5;
            robot.WorkTime = 30;
            robots.Add(robot);
            */


            return robots;
        }

        private Robot generateRobot()
        {
            var robot = new Robot();
            robot.Color = Color.FromArgb(m_rand.Next(255), m_rand.Next(255), m_rand.Next(255));
            robot.DistanceCost = 1; //m_rand.Next(10) + 10;
            robot.WorkCost = 1; //m_rand.Next(10) + 10;
            robot.DistanceTime = 5; //m_rand.Next(10) + 100;
            robot.WorkTime = 5; // m_rand.Next(10) + 100;
            return robot;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Params;
using MathNet.Numerics.LinearAlgebra;

namespace Trajectories
{
    /// <summary>
    /// Training creation according to Training creator.
    /// It's method for 'Create' called each trial by it's handler.
    /// </summary>
    public class Training : ITrajectoryCreator
    {
        #region ATTRIBUTES
        /// <summary>
        /// Describes the duration to make the move.
        /// </summary>
        private double _duration;

        /// <summary>
        /// The variables readen from the xlsx protocol file.
        /// </summary>
        private Variables _variablesList;

        /// <summary>
        /// Final list holds all the current cross varying vals by dictionary of variables with values for each line(trial) for ratHouseParameters.
        /// </summary>
        private List<Dictionary<string, double>> _crossVaryingVals;

        /// <summary>
        /// The static variables list in double value presentation.
        /// The string is for the variable name.
        /// </summary>
        private Dictionary<string, double> _staticVars;

        /// <summary>
        /// The numbers of samples for each trajectory.
        /// </summary>
        private int _frequency;

        /// <summary>
        /// The Matlab handler object.
        /// </summary>
        private MLApp.MLApp _matlabApp;

        /// <summary>
        /// Indicates if to draw or not the movement graph for each trial.
        /// </summary>
        public bool DrawTrialMovementGraph { get; set; }
        #endregion ATTRIBUTES

        #region CONSTRUCTORS
        /// <summary>
        /// Defaulte Constructor.
        /// </summary>
        public Training()
        {

        }

        /// <summary>
        /// Training Constructor.
        /// </summary>
        /// <param name="variablesList">The variables list showen in the readen from the excel and changed by the main gui.</param>
        /// <param name="crossVaryingVals">Final list holds all the current cross varying vals by dictionary of variables with values for each line(trial) for ratHouseParameters.</param>
        /// <param name="trajectorySampleNumber">The number of sample points for the trajectory.</param>
        public Training(MLApp.MLApp matlabApp, Variables variablesList, List<Dictionary<string, double>> crossVaryingVals, Dictionary<string, double> staticVals, int trajectorySampleNumber)
        {
            _matlabApp = matlabApp;
            _variablesList = variablesList;
            _crossVaryingVals = crossVaryingVals;
            _staticVars = staticVals;
            _frequency = trajectorySampleNumber;
        }
        #endregion CONSTRUCTORS

        #region FUNCTIONS
        /// <summary>
        /// Generating a vector of sampled gaussian cdf with the given attributes.
        /// </summary>
        /// <param name="duration">The duraon for the trajectory.</param>
        /// <param name="sigma">The number of sigmas for the trajectory in the generated gayssian cdf.</param>
        /// <param name="magnitude">The mfgnitude of the trajectory.</param>
        /// <param name="frequency">The number of samples for the gaussian cdf to the trajectory.</param>
        /// <returns>
        /// The sampled gaussian cdf trajector.
        /// The vector length is as the fgiven frequency.
        /// </returns>
        public Vector<double> GenerateGaussianSampledCDF(double duration, double sigma, double magnitude, int frequency)
        {
            Vector<double> returnedVector = CreateVector.Dense<double>(frequency * (int)duration,0);
            //MatlabPlotFunction(returnedVector);
            return returnedVector;
        }

        /// <summary>
        /// Computes the trajectoy tuple (for the ratHouseTrajectory and for the landscapeHouseTrajectory).
        /// </summary>
        /// <param name="index">The index from the crossVaryingList to take the attributes of he varying variables from.</param>
        /// <returns>The trajectory tuple (for the ratHouseTrajectory and for the landscapeHouseTrajectory). </returns>
        public Tuple<Trajectory2, Trajectory2> CreateTrialTrajectory(int index)
        {
            //reading the needed current trial parameters into the object members.
            ReadTrialParameters(index);

            Trajectory2 ratHouseTrajectory = new Trajectory2()
            {
                X = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                Y = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                Z = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                //rx - roll , ry - pitch , rz = yaw
                RX = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                RY = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                RZ = CreateVector.Dense<double>((int)(_frequency * _duration), 0)
            };


            Trajectory2 landscapeHouseTrajectory = new Trajectory2()
            {
                X = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                Y = CreateVector.Dense<double>((int)(_frequency * _duration), 0),

                Z = CreateVector.Dense<double>((int)(_frequency * _duration), 0),
                //rx - roll , ry - pitch , rz = yaw
                RX = CreateVector.Dense<double>(_frequency, 0),
                RY = CreateVector.Dense<double>(_frequency, 0),
                RZ = CreateVector.Dense<double>(_frequency, 0)
            };

            //if need to plot the trajectories
            if (DrawTrialMovementGraph)
            {
                MatlabPlotTrajectoryFunction(ratHouseTrajectory);
            }

            return new Tuple<Trajectory2, Trajectory2>(ratHouseTrajectory, landscapeHouseTrajectory);
        }

        /// <summary>
        /// Read the current trial needed parameters and insert them to the object members.
        /// </summary>
        public void ReadTrialParameters(int index)
        {
            Dictionary<string, double> currentVaryingTrialParameters = _crossVaryingVals[index];

            if (_staticVars.ContainsKey("STIMULUS_DURATION"))
                _duration = _staticVars["STIMULUS_DURATION"];
            else if (_crossVaryingVals[index].Keys.Contains("STIMULUS_DURATION"))
            {
                _duration = currentVaryingTrialParameters["STIMULUS_DURATION"];
            }
        }

        /// <summary>
        /// Plotting a vector into  new window for 2D function with MATLAB.
        /// </summary>
        /// <param name="drawingVector">
        /// The vector to be drawn into the graph.
        /// The x axis is the size of the vecor.
        /// The y axis is the vector.
        /// </param>
        public void MatlabPlotFunction(Vector<double> drawingVector)
        {
            double[] dArray = ConvertVectorToArray(drawingVector);
            _matlabApp.Execute("figure;");
            _matlabApp.Execute("title('Trajectories')");
            _matlabApp.Execute("plot(drawingVector)");

        }

        /// <summary>
        /// Plotting all 6 attributes for the given trajectory.
        /// </summary>
        /// <param name="traj">The trajectory to be decomposed to it's 6 components and to plot in a figure.</param>
        public void MatlabPlotTrajectoryFunction(Trajectory2 traj)
        {

            _matlabApp.Execute("figure;");
            _matlabApp.Execute("title('Trajectories')");

            _matlabApp.PutWorkspaceData("rows", "base", (double)3);
            _matlabApp.PutWorkspaceData("columns", "base", (double)2);

            double[] dArray = ConvertVectorToArray(traj.X);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "x");
            _matlabApp.PutWorkspaceData("index", "base", (double)1);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");

            dArray = ConvertVectorToArray(traj.Y);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "y");
            _matlabApp.PutWorkspaceData("index", "base", (double)2);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");

            dArray = ConvertVectorToArray(traj.Z);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "z");
            _matlabApp.PutWorkspaceData("index", "base", (double)3);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");

            dArray = ConvertVectorToArray(traj.RX);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "rx");
            _matlabApp.PutWorkspaceData("index", "base", (double)4);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");

            dArray = ConvertVectorToArray(traj.RY);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "ry");
            _matlabApp.PutWorkspaceData("index", "base", (double)5);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");

            dArray = ConvertVectorToArray(traj.RZ);
            _matlabApp.PutWorkspaceData("drawingVector", "base", dArray);
            _matlabApp.PutWorkspaceData("subplotGraphName", "base", "rz");
            _matlabApp.PutWorkspaceData("index", "base", (double)6);
            _matlabApp.Execute("subplot(rows , columns , index)");
            _matlabApp.Execute("plot(drawingVector)");
            _matlabApp.Execute("title(subplotGraphName)");
        }

        /// <summary>
        /// Converts a double vector type to double array type.
        /// </summary>
        /// <param name="vector">The vector to be converted.</param>
        /// <returns>The converted array.</returns>
        public double[] ConvertVectorToArray(Vector<double> vector)
        {
            double[] dArray = new double[vector.Count];
            for (int i = 0; i < dArray.Length; i++)
                dArray[i] = vector[i];

            return dArray;
        }
        #endregion FUNCTIONS
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
//Classe pour faire un DTW simple, à utiliser comme ceci

        //SimpleDTW dtw = new SimpleDTW(MonPremierList<Vecto3>,MonDeuxiemeList<Vector3>);
        //dtw.computeDTW();
        //dtw.getSum(); donne la valeur du dtw

namespace DTW
{
    class SimpleDTW
    {
        private double[] x1, y1, z1, x2, y2, z2;
        double[,] distance;
        double[,] f;
        private List<Vector3> Traj1;
        private List<Vector3> Traj2;
        ArrayList pathX;
        ArrayList pathY;
        ArrayList distanceList;
        double sum;

        public SimpleDTW(List<Vector3> _Traj1, List<Vector3> _Traj2, int n = 1000)
        {
            Traj1 = new List<Vector3>(_Traj1);
            Traj2 = new List<Vector3>(_Traj2);
            Traj1 = resample(Traj1,n);
            Traj2 = resample(Traj2,n);

            //exctraction of x,y and z element of each point of the vector3 lists
            x1 = new double[Traj1.Count];
            y1 = new double[Traj1.Count];
            z1 = new double[Traj1.Count];
            x2 = new double[Traj2.Count];
            y2 = new double[Traj2.Count];
            z2 = new double[Traj2.Count];
                      
            for (int i = 0; i < Traj1.Count; i++)
            {
                x1[i] = Traj1[i].x;
                y1[i] = Traj1[i].y;
                z1[i] = Traj1[i].z;
            }

            for (int i = 0; i < Traj2.Count; i++)
            {
                x2[i] = Traj2[i].x;
                y2[i] = Traj2[i].y;
                z2[i] = Traj2[i].z;
            }

            distance = new double[Traj1.Count, Traj2.Count];
            f = new double[Traj1.Count+1, Traj2.Count+1];

            for (int i = 0; i < Traj1.Count; ++i)
            {
                for (int j = 0; j < Traj2.Count; ++j)
                {
                    distance[i, j] = Math.Sqrt((Math.Pow(x1[i] - x2[j], 2) + Math.Pow(y1[i] - y2[j], 2))+ Math.Pow(z1[i] - z2[j], 2));
                }
            }

            for (int i = 0; i <= Traj1.Count; ++i)
            {
                for (int j = 0; j <= Traj2.Count; ++j)
                {
                    f[i, j] = -1.0;
                }
            }

            for (int i = 1; i <= Traj1.Count; ++i) {
                f[i,0] = double.PositiveInfinity;
            }
            for (int j = 1; j <= Traj2.Count; ++j) {
                f[0, j] = double.PositiveInfinity;
            }

            f[0, 0] = 0.0;
            sum = 0.0;

            pathX = new ArrayList();
            pathY = new ArrayList();
            distanceList = new ArrayList();
        }

        public ArrayList getPathX(){
            return pathX;
        }

        public ArrayList getPathY() {
            return pathY;
        }

        public double getSum(){
            return sum;
        }

        public double[,] getFMatrix() {
            return f;
        }

        public ArrayList getDistanceList() {
            return distanceList;
        }

        public void computeDTW() {
            sum = computeFBackward(Traj1.Count, Traj2.Count);
            //sum = computeFForward();
        }

        public double computeFForward() {
            for (int i = 1; i <= Traj1.Count; ++i) {
                for (int j = 1; j <= Traj2.Count; ++j) {
                    if (f[i - 1, j] <= f[i - 1, j - 1] && f[i - 1, j] <= f[i, j - 1]) {
                        f[i, j] = distance[i - 1, j - 1] + f[i - 1, j];
                    }
                    else if (f[i, j - 1] <= f[i - 1, j - 1] && f[i, j - 1] <= f[i - 1, j]) {
                        f[i, j] = distance[i - 1, j - 1] + f[i, j - 1 ];                    
                    }
                    else if (f[i - 1, j-1 ] <= f[i , j - 1] && f[i - 1, j - 1] <= f[i-1, j ]) {
                        f[i, j] = distance[i - 1, j - 1] + f[i - 1, j - 1 ];
                    }
                }
            }
            return f[Traj1.Count, Traj2.Count];
        }

        public double computeFBackward(int i, int j)
        {
            if (!(f[i, j] < 0.0) ){
                return f[i, j];
            }
            else {
                if (computeFBackward(i - 1, j) <= computeFBackward(i, j - 1) && computeFBackward(i - 1, j) <= computeFBackward(i - 1, j - 1)
                    && computeFBackward(i - 1, j) < double.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + computeFBackward(i - 1, j);
                }
                else if (computeFBackward(i, j - 1) <= computeFBackward(i - 1, j) && computeFBackward(i, j - 1) <= computeFBackward(i - 1, j - 1)
                    && computeFBackward(i, j - 1) < double.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + computeFBackward(i, j - 1);    
                }
                else if (computeFBackward(i - 1, j - 1) <= computeFBackward(i - 1, j) && computeFBackward(i - 1, j - 1) <= computeFBackward(i, j - 1)
                    && computeFBackward(i - 1, j - 1) < double.PositiveInfinity)
                {
                    f[i, j] = distance[i - 1, j - 1] + computeFBackward(i - 1, j - 1);
                }
            }
            return f[i, j];
        }
        public List<Vector3> resample(List<Vector3> Traj, int n)
        {
            // resample the trajectory with n number of equidistant points
            List<Vector3> NewTraj = new List<Vector3>(n);
            List<Vector3> OldTraj = new List<Vector3>(n);

            float pathLength = PathLength(Traj)/ (n - 1);

            float D = 0.0f;
            int i = 1;
            while (i < Traj.Count)
            {   

                float d = GetDistanceVec(Traj[i],Traj[i-1]);
                if (D + d >= pathLength)
                {
                    float deltaDistance = (pathLength - D) / d;
                    Vector3 newVec = new Vector3(
                        Traj[i-1].x + deltaDistance * (Traj[i].x - Traj[i-1].x),
                        Traj[i-1].y + deltaDistance * (Traj[i].y - Traj[i-1].y),
                        Traj[i-1].z + deltaDistance * (Traj[i].z - Traj[i-1].z)
                    );

                    NewTraj.Add(newVec);
                    Traj.Insert(i,newVec);
                    D = 0.0f;
                }
                else
                {
                    D += d;
                }
                i++;
            }
            
            if (NewTraj.Count == n - 1) // Fix a possible roundoff error
            {
                NewTraj.Add(Traj[Traj.Count - 1]);
            }
            Traj = OldTraj;
            return NewTraj;
        }

        public static float PathLength(List<Vector3> Trajectoire)
        {
            float pathLength = 0.0f;
            int i = 1;
            while (i < Trajectoire.Count)
            {
                pathLength = pathLength + GetDistanceVec(Trajectoire[i],Trajectoire[i-1]);
                i++;
            }
            return pathLength;
        }

        public static float GetDistanceVec(Vector3 Vec1, Vector3 Vec2)
        {
            return (float)Math.Sqrt((Math.Pow(Vec1.x - Vec2.x, 2) + Math.Pow(Vec1.y - Vec2.y, 2))+ Math.Pow(Vec1.z - Vec2.z, 2));
        }


    }
}
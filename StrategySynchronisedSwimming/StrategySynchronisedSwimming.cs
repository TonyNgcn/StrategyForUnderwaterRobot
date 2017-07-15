using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;
using URWPGSim2D.StrategyHelper;
using System.IO;

namespace URWPGSim2D.Strategy
{
    public class Strategy : MarshalByRefObject, IStrategy
    {
        #region reserved code never be changed or removed
        /// <summary>
        /// override the InitializeLifetimeService to return null instead of a valid ILease implementation
        /// to ensure this type of remote object never dies
        /// </summary>
        /// <returns>null</returns>
        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null; // makes the object live indefinitely
        }
        #endregion

        /// <summary>
        /// 决策类当前对象对应的仿真使命参与队伍的决策数组引用 第一次调用GetDecision时分配空间
        /// </summary>
        private Decision[] decisions = null;
        /// <summary>
        /// 获取队伍名称 在此处设置参赛队伍的名称
        /// </summary>
        /// <returns>队伍名称字符串</returns>
        public string GetTeamName()
        {
            return "Team First";
        }

        //public static int isDirectionRight(float a, float b)
        //{
        //    if ((Math.Abs(a - b) < 0.1) || (Math.Abs(a + (float)Math.PI - b) < 0.1)
        //        return 0;

        //    else return 1;
        //    //if (a - b > 0.1) return 1;//a在b右边
        //    //else if (a - b < -0.1) return -1; //a在b左边
        //}

        public static int isDirectionRight(float a, float b)
        {
            if (a > Math.PI) a -= (float)(2 * Math.PI);
            if (b > Math.PI) b -= (float)(2 * Math.PI);
            if (a < -Math.PI) a += (float)(2 * Math.PI);
            if (b < -Math.PI) b += (float)(2 * Math.PI);
            if (a - b > 0.1) return 1;//a在b右边
            else if (a - b < -0.1) return -1; //a在b左边
            else return 0;
        }
        public static bool allEqual(int[] group, int value, int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                if (group[i] != value)
                {
                    return false;
                }
            }
            return true;
        }
        public static void stopFish(ref Decision decision, int i)
        {
            decision.VCode = 0;
            decision.TCode = 7;
            timeForPoseToPose[i] = 0;
        }
        public static float getVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2d) + Math.Pow((a.Z - b.Z), 2d)));
        }
        public static bool complete = false;
        Decision[] preDecisions = null;
        private static int flag = 0;//主函数标志值
        private static int timeflag = 0;
        private static int[] timeForPoseToPose = new int[11];
        //private int remainRecord = 0;
        //private int[] zeroflag = new int[10];
        //private int[] flyflag = new int[10];
        private static int[] hillflag = new int[11];
        private static int[] oneflag = new int[11];
        private static int[] playflag = new int[11];
        private static int[] circleflag = new int[11];
        /// <summary>
        /// 获取当前仿真使命（比赛项目）当前队伍所有仿真机器鱼的决策数据构成的数组
        /// </summary>
        /// <param name="mission">服务端当前运行着的仿真使命Mission对象</param>
        /// <param name="teamId">当前队伍在服务端运行着的仿真使命中所处的编号 
        /// 用于作为索引访问Mission对象的TeamsRef队伍列表中代表当前队伍的元素</param>
        /// <returns>当前队伍所有仿真机器鱼的决策数据构成的Decision数组对象</returns>
        #region 旧代码
        /* #region 心形函数

      public void Heart(ref Mission mission, int teamId, ref Decision[] decision)
         {
             #region 声明变量
             int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
             int[] times = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };//PoseToPose函数所需参数
             #region 一堆鱼
             RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
             RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
             RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
             RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
             RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
             RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
             RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
             RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
             RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
             #endregion
             #endregion
             #region 构成心形的目标点
             xna.Vector3 Point1 = new xna.Vector3(-144, 0, -300);
             xna.Vector3 Point2 = new xna.Vector3(186, 0, -540);
             xna.Vector3 Point3 = new xna.Vector3(-186, 0, 228);
             xna.Vector3 Point4 = new xna.Vector3(-612, 0, 180);
             xna.Vector3 Point5 = new xna.Vector3(510, 0, -546);
             xna.Vector3 Point6 = new xna.Vector3(450, 0, -282);
             xna.Vector3 Point7 = new xna.Vector3(-942, 0, -654);
             xna.Vector3 Point8 = new xna.Vector3(-498, 0, -384);
             #endregion
             #region 一堆鱼使用PoseToPose函数游到指定位置
             Helpers.PoseToPose(ref decision[1], fish2, Point1, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[0]);
             Helpers.PoseToPose(ref decision[2], fish3, Point2, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[1]);
             Helpers.PoseToPose(ref decision[3], fish4, Point3, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[2]);
             Helpers.PoseToPose(ref decision[4], fish5, Point4, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[3]);
             Helpers.PoseToPose(ref decision[5], fish6, Point5, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[4]);
             Helpers.PoseToPose(ref decision[6], fish7, Point6, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[5]);
             Helpers.PoseToPose(ref decision[7], fish8, Point7, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[6]);
             Helpers.PoseToPose(ref decision[8], fish9, Point8, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[7]);
             //Helpers.PoseToPose(ref decision[9], fish10, Point9, (float)(Math.PI), 30.0f, 150, msPerCycle, ref times[8]);
             #endregion;
         }
         #endregion

        #region 0形函数
        public void Zero(ref Mission mission,int teamId,ref Decision[] decisions)
         {
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
             RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
             RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
             RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
             RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
             RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
             RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
             RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
             RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
             RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成数字0的目标点
            xna.Vector3 A1 = new xna.Vector3(-12, 0, -696);
            xna.Vector3 A2 = new xna.Vector3(192, 0, -258);
            xna.Vector3 A3 = new xna.Vector3(210, 0, 240);
            xna.Vector3 A4 = new xna.Vector3(60, 0, 782);
            xna.Vector3 A5 = new xna.Vector3(-432, 0, 726);
            xna.Vector3 A6 = new xna.Vector3(-582, 0, 222);
            xna.Vector3 A7 = new xna.Vector3(-516, 0, -276);
            xna.Vector3 A8 = new xna.Vector3(-264, 0, -726);
            #endregion
            #region 构成数字0的目标角度
            float AD1 = (float)-2.1991;
            float AD2 = (float)-1.8326;
            float AD3 = (float)-1.2915;
            float AD4 = (float)-0.8727;
            float AD5 = (float)-2.09434;
            float AD6 = (float)-1.74524;
            float AD7 = (float)-1.29154;
            float AD8 = (float)-0.95984;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            #region 一堆鱼使用Dribble函数游到指定位置
            if (zeroflag[1] == 0) Helpers.Dribble(ref decisions[1], fish2, A1, AD1, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[2] == 0) Helpers.Dribble(ref decisions[2], fish3, A2, AD2, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[3] == 0) Helpers.Dribble(ref decisions[3], fish4, A3, AD3, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[4] == 0) Helpers.Dribble(ref decisions[4], fish5, A4, AD4, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[5] == 0) Helpers.Dribble(ref decisions[5], fish6, A5, AD5, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[6] == 0) Helpers.Dribble(ref decisions[6], fish7, A6, AD6, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[7] == 0) Helpers.Dribble(ref decisions[7], fish8, A7, AD7, 30, 15, 120, 14, 9, 10, msPerCycle, true);
            if (zeroflag[8] == 0) Helpers.Dribble(ref decisions[8], fish9, A8, AD8, 30, 15, 120, 14, 9, 10, msPerCycle, true);

            Helpers.Dribble(ref decisions[9], fish10, fish1Location2, (float)2.0769, 30, 30, 30, 14, 12, 5, msPerCycle, true);
            #endregion;
            #region 判断全部鱼在指定位置,完成后flag=1
            if (getVectorDistance(A1, fish2Location) < 200) { decisions[1].VCode = 1; zeroflag[1] = 1; if (isDirectionRight(AD1, fish2Direction) == 0) { decisions[1].TCode = 0; decisions[1].VCode = 0; zeroflag[1] = 2; } else if (isDirectionRight(AD1, fish2Direction) == 1) decisions[1].TCode = 8; else decisions[1].TCode = 6; }
            if (getVectorDistance(A2, fish3Location) < 200) { decisions[2].VCode = 1; zeroflag[2] = 1; if (isDirectionRight(AD2, fish3Direction) == 0) { decisions[2].TCode = 0; decisions[2].VCode = 0; zeroflag[2] = 2; } else if (isDirectionRight(AD2, fish3Direction) == 1) decisions[2].TCode = 8; else decisions[2].TCode = 6; }
            if (getVectorDistance(A3, fish4Location) < 200) { decisions[3].VCode = 1; zeroflag[3] = 1; if (isDirectionRight(AD3, fish4Direction) == 0) { decisions[3].TCode = 0; decisions[3].VCode = 0; zeroflag[3] = 2; } else if (isDirectionRight(AD3, fish4Direction) == 1) decisions[3].TCode = 8; else decisions[3].TCode = 6; }
            if (getVectorDistance(A4, fish5Location) < 200) { decisions[4].VCode = 1; zeroflag[4] = 1; if (isDirectionRight(AD4, fish5Direction) == 0) { decisions[4].TCode = 0; decisions[4].VCode = 0; zeroflag[4] = 2; } else if (isDirectionRight(AD4, fish5Direction) == 1) decisions[4].TCode = 8; else decisions[4].TCode = 6; }
            if (getVectorDistance(A5, fish6Location) < 200) { decisions[5].VCode = 1; zeroflag[5] = 1; if (isDirectionRight(AD5, fish6Direction) == 0) { decisions[5].TCode = 0; decisions[5].VCode = 0; zeroflag[5] = 2; } else if (isDirectionRight(AD5, fish6Direction) == 1) decisions[5].TCode = 8; else decisions[5].TCode = 6; }
            if (getVectorDistance(A6, fish7Location) < 200) { decisions[6].VCode = 1; zeroflag[6] = 1; if (isDirectionRight(AD6, fish7Direction) == 0) { decisions[6].TCode = 0; decisions[6].VCode = 0; zeroflag[6] = 2; } else if (isDirectionRight(AD6, fish7Direction) == 1) decisions[6].TCode = 8; else decisions[6].TCode = 6; }
            if (getVectorDistance(A7, fish8Location) < 200) { decisions[7].VCode = 1; zeroflag[7] = 1; if (isDirectionRight(AD7, fish8Direction) == 0) { decisions[7].TCode = 0; decisions[7].VCode = 0; zeroflag[7] = 2; } else if (isDirectionRight(AD7, fish8Direction) == 1) decisions[7].TCode = 8; else decisions[7].TCode = 6; }
            if (getVectorDistance(A8, fish9Location) < 200) { decisions[8].VCode = 1; zeroflag[8] = 1; if (isDirectionRight(AD8, fish9Direction) == 0) { decisions[8].TCode = 0; decisions[8].VCode = 0; zeroflag[8] = 2; } else if (isDirectionRight(AD8, fish9Direction) == 1) decisions[8].TCode = 8; else decisions[8].TCode = 6; }

            if (getVectorDistance(A1, fish2Location) > 120) zeroflag[1] = 0;
            if (getVectorDistance(A2, fish3Location) > 120) zeroflag[2] = 0;
            if (getVectorDistance(A3, fish4Location) > 120) zeroflag[3] = 0;
            if (getVectorDistance(A4, fish5Location) > 120) zeroflag[4] = 0;
            if (getVectorDistance(A5, fish6Location) > 120) zeroflag[5] = 0;
            if (getVectorDistance(A6, fish7Location) > 120) zeroflag[6] = 0;
            if (getVectorDistance(A7, fish8Location) > 120) zeroflag[7] = 0;
            if (getVectorDistance(A8, fish9Location) > 120) zeroflag[8] = 0;



            if (timeflag != 1 && zeroflag[1] == 2 && zeroflag[2] == 2 && zeroflag[3] == 2 && zeroflag[4] == 2 && zeroflag[5] == 2 && zeroflag[6] == 2 && zeroflag[7] == 2 && zeroflag[8] == 2)  
                timeflag ++;
           
            if (timeflag>=1)
            {
                timeflag ++;
            }
            if (zeroflag[1] == 1 || zeroflag[2] == 1 || zeroflag[3] == 1 || zeroflag[4] == 1 || zeroflag[5] == 1 || zeroflag[6] == 1 || zeroflag[7] == 1 || zeroflag[8] == 1) 
            {
                StreamWriter log = new StreamWriter("C:\\Users\\wujun\\Desktop\\URWPGSim2D\\URWPGSim2D\\Strategy\\log.txt", true);
                for (int i = 1; i < 10; i++)
                    log.WriteLine(i+'='+mission.TeamsRef[teamId].Fishes[i].BodyDirectionRad);
                log.Close();
            }

            if (timeflag >= 30)
            {
                timeflag = 0;
                flag = 1;
            }
            #endregion
        }
        #endregion
         #region 飞字函数
          public void FlyCharacter(ref Mission mission, int teamId, ref Decision[] decisions)
          {
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
            RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
            RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
            RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
            RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
            RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成飞字的目标点
            xna.Vector3 F2 = new xna.Vector3(870, 0, -216);
            xna.Vector3 F3 = new xna.Vector3(816, 0, 222);
            xna.Vector3 F4 = new xna.Vector3(-588, 0, -622);
            xna.Vector3 F5 = new xna.Vector3(1044, 0, 894);
            xna.Vector3 F6 = new xna.Vector3(630, 0, 570);
            xna.Vector3 F7 = new xna.Vector3(480, 0, 48);
            xna.Vector3 F8 = new xna.Vector3(-60, 0, -618);
            xna.Vector3 F9 = new xna.Vector3(348, 0, -414);
            #endregion
            #region 构成飞字的目标角度
            float FD2 = (float)-0.9774;
            float FD3 = (float)-0.8727;
            float FD4 = (float)3.05;
            float FD5 = (float)2.618;
            float FD6 = (float)-2.0071;
            float FD7 = (float)-1.85;
            float FD8 = (float)-2.7925;
            float FD9 = (float)-1.8326;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            #region 一堆鱼用Dribble到达指定地点
            if (flyflag[2] == 0) Helpers.Dribble(ref decisions[2], fish3, F2, FD2, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[3] == 0) Helpers.Dribble(ref decisions[3], fish4, F3, FD3, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[4] == 0) Helpers.Dribble(ref decisions[4], fish5, F4, FD4, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[5] == 0) Helpers.Dribble(ref decisions[5], fish6, F5, FD5, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[6] == 0) Helpers.Dribble(ref decisions[6], fish7, F6, FD6, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[7] == 0) Helpers.Dribble(ref decisions[7], fish8, F7, FD7, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[8] == 0) Helpers.Dribble(ref decisions[8], fish9, F8, FD8, 10, 5, 90, 14, 9, 18, msPerCycle, true);
            if (flyflag[9] == 0) Helpers.Dribble(ref decisions[9], fish10, F9, FD9, 10, 5, 90, 14, 9, 18, msPerCycle, true);

            //针对黄鱼，逼到左下角
            Helpers.Dribble(ref decisions[1], fish2, fish1Location2, (float)2.0769, 20, 10, 100, 14, 12, 5, msPerCycle, true);
            #endregion
            #region 判断鱼是否到达目标点
            if (isDirectionRight(fish3Direction, FD2) == 0 && getVectorDistance(F2, fish3Location) < 45) { decisions[2].VCode = 0; flyflag[2] = 1; decisions[2].TCode = 7; }
            if (isDirectionRight(fish4Direction, FD3) == 0 && getVectorDistance(F3, fish4Location) < 45) { decisions[3].VCode = 0; flyflag[3] = 1; decisions[3].TCode = 7; }
            if (isDirectionRight(fish5Direction, FD4) == 0 && getVectorDistance(F4, fish5Location) < 45) { decisions[4].VCode = 0; flyflag[4] = 1; decisions[4].TCode = 7; }
            if (isDirectionRight(fish6Direction, FD5) == 0 && getVectorDistance(F5, fish6Location) < 45) { decisions[5].VCode = 0; flyflag[5] = 1; decisions[5].TCode = 7; }
            if (isDirectionRight(fish7Direction, FD6) == 0 && getVectorDistance(F6, fish7Location) < 45) { decisions[6].VCode = 0; flyflag[6] = 1; decisions[6].TCode = 7; }
            if (isDirectionRight(fish8Direction, FD7) == 0 && getVectorDistance(F7, fish8Location) < 45) { decisions[7].VCode = 0; flyflag[7] = 1; decisions[7].TCode = 7; }
            if (isDirectionRight(fish9Direction, FD8) == 0 && getVectorDistance(F8, fish9Location) < 45) { decisions[8].VCode = 0; flyflag[8] = 1; decisions[8].TCode = 7; }
            if (isDirectionRight(fish10Direction, FD9) == 0 && getVectorDistance(F9, fish9Location) < 45) { decisions[9].VCode = 0; flyflag[9] = 1; decisions[9].TCode = 7; }

            if (getVectorDistance(F2, fish3Location) > 60) flyflag[2] = 0;
            if (getVectorDistance(F3, fish4Location) > 60) flyflag[3] = 0;
            if (getVectorDistance(F4, fish5Location) > 60) flyflag[4] = 0;
            if (getVectorDistance(F5, fish6Location) > 60) flyflag[5] = 0;
            if (getVectorDistance(F6, fish7Location) > 60) flyflag[6] = 0;
            if (getVectorDistance(F7, fish8Location) > 60) flyflag[7] = 0;
            if (getVectorDistance(F8, fish9Location) > 60) flyflag[8] = 0;
            if (getVectorDistance(F9, fish10Location) > 60) flyflag[9] = 0;

            #endregion
        }
        #endregion
          */
        #endregion
        #region 山字
        public static void hillCharacter(ref Mission mission, int teamId, ref Decision[] decisions)
         {
            //StreamWriter log = new StreamWriter("C:\\Users\\wujun\\Desktop\\URWPGSim2D\\URWPGSim2D\\Strategy\\log.txt", true);
            //log.Write(allEqual(hillflag, 1, 2, 10));
            //log.Write(' ');
            //log.WriteLine("end");
            //log.Close();
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
            RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
            RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
            RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
            RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
            RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成山字的目标点
            xna.Vector3 hill21 = new xna.Vector3(-1404, 0, 516);
            xna.Vector3 hill3 = new xna.Vector3(-726, 0, 570);
            xna.Vector3 hill4 = new xna.Vector3(108, 0, -702);
            xna.Vector3 hill5 = new xna.Vector3(108, 0, -156);
            xna.Vector3 hill6 = new xna.Vector3(108, 0, 534);
            xna.Vector3 hill7 = new xna.Vector3(-390, 0, 996);
            xna.Vector3 hill8 = new xna.Vector3(216, 0, 996);
            xna.Vector3 hill9 = new xna.Vector3(834, 0, 996);
            xna.Vector3 hill10 = new xna.Vector3(948, 0, 564);
            xna.Vector3 hill22 = new xna.Vector3(204, 0, -1122);
            xna.Vector3 hill23 = new xna.Vector3(1644, 0, 648);
            #endregion
            #region 构成山字的目标角度
            float HD21 = (float)-1.0472;
            float HD3 = (float)-1.5708;
            float HD4 = (float)-1.5708;
            float HD5 = (float)-1.5708;
            float HD6 = (float)-1.5708;
            float HD7 = 0;
            float HD8 = 0;
            float HD9 = 0;
            float HD10 = (float)-1.5708;
            float HD22 = 0;
            float HD23 = (float)0.7854;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            //xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            #region 一堆鱼使用PoseToPose函数游到指定位置
            if (hillflag[2] == 0) Helpers.PoseToPose(ref decisions[1], fish2, hill21, HD21, 40f, 200f, msPerCycle, ref timeForPoseToPose[2]);
            if (hillflag[3] == 0) Helpers.PoseToPose(ref decisions[2], fish3, hill3, HD3, 40f, 200f, msPerCycle, ref timeForPoseToPose[3]);
            if (hillflag[4] == 0) Helpers.PoseToPose(ref decisions[3], fish4, hill4, HD4, 40f, 200f, msPerCycle, ref timeForPoseToPose[4]);
            if (hillflag[5] == 0) Helpers.PoseToPose(ref decisions[4], fish5, hill5, HD5, 40f, 200f, msPerCycle, ref timeForPoseToPose[5]);
            if (hillflag[6] == 0) Helpers.PoseToPose(ref decisions[5], fish6, hill6, HD6, 40f, 200f, msPerCycle, ref timeForPoseToPose[6]);
            if (hillflag[7] == 0) Helpers.PoseToPose(ref decisions[6], fish7, hill7, HD7, 40f, 200f, msPerCycle, ref timeForPoseToPose[7]);
            if (hillflag[8] == 0) Helpers.PoseToPose(ref decisions[7], fish8, hill8, HD8, 40f, 200f, msPerCycle, ref timeForPoseToPose[8]);
            if (hillflag[9] == 0) Helpers.PoseToPose(ref decisions[8], fish9, hill9, HD9, 40f, 200f, msPerCycle, ref timeForPoseToPose[9]);
            if (hillflag[10] == 0) Helpers.PoseToPose(ref decisions[9], fish10, hill10, HD10, 40f, 200f, msPerCycle, ref timeForPoseToPose[10]);
            #endregion;
            #region 判断是否到达目标点
            if (hillflag[0] == 0 && getVectorDistance(hill21, fish2Location) < 200 && isDirectionRight(HD21, fish2Direction) == 0) { hillflag[2] = 1; stopFish(ref decisions[1], 2); }
            if (getVectorDistance(hill3, fish3Location) < 200 && isDirectionRight(HD3, fish3Direction) == 0) { hillflag[3] = 1; stopFish(ref decisions[2], 3); }
            if (getVectorDistance(hill4, fish4Location) < 200 && isDirectionRight(HD4, fish4Direction) == 0) { hillflag[4] = 1; stopFish(ref decisions[3], 4); }
            if (getVectorDistance(hill5, fish5Location) < 200 && isDirectionRight(HD5, fish5Direction) == 0) { hillflag[5] = 1; stopFish(ref decisions[4], 5); }
            if (getVectorDistance(hill6, fish6Location) < 200 && isDirectionRight(HD6, fish6Direction) == 0) { hillflag[6] = 1; stopFish(ref decisions[5], 6); }
            if (getVectorDistance(hill7, fish7Location) < 200 && isDirectionRight(HD7, fish7Direction) == 0) { hillflag[7] = 1; stopFish(ref decisions[6], 7); }
            if (getVectorDistance(hill8, fish8Location) < 200 && isDirectionRight(HD8, fish8Direction) == 0) { hillflag[8] = 1; stopFish(ref decisions[7], 8); }
            if (getVectorDistance(hill9, fish9Location) < 200 && isDirectionRight(HD9, fish9Direction) == 0) { hillflag[9] = 1; stopFish(ref decisions[8], 9); }
            if (getVectorDistance(hill10, fish10Location) < 200 && isDirectionRight(HD10, fish10Direction) == 0) { hillflag[10] = 1; stopFish(ref decisions[9], 10); }
            #endregion
            #region 如果达到目标点后被破坏需要修正
            if (hillflag[0] == 0 && getVectorDistance(hill21, fish2Location) > 200)  hillflag[2] = 0;
            if (getVectorDistance(hill3, fish3Location) > 200) hillflag[3] = 0;
            if (getVectorDistance(hill4, fish4Location) > 200) hillflag[4] = 0;
            if (getVectorDistance(hill5, fish5Location) > 200) hillflag[5] = 0;
            if (getVectorDistance(hill6, fish6Location) > 200) hillflag[6] = 0;
            if (getVectorDistance(hill7, fish7Location) > 200) hillflag[7] = 0;
            if (getVectorDistance(hill8, fish8Location) > 200) hillflag[8] = 0;
            if (getVectorDistance(hill9, fish9Location) > 200) hillflag[9] = 0;
            if (getVectorDistance(hill10, fish10Location) > 200) hillflag[10] = 0;
            #endregion
            #region 山字第二阶段
            if (hillflag[0] == 0 && allEqual(hillflag, 1, 2, 10))
            {
                hillflag[0] = 1;
                timeForPoseToPose[2] = 0;
            }
            if (hillflag[0] == 1) Helpers.PoseToPose(ref decisions[1], fish2, hill22, HD22, 40f, 200f, msPerCycle, ref timeForPoseToPose[2]);
            if (getVectorDistance(hill22, fish2Location) < 200)  { hillflag[0] = 2; stopFish(ref decisions[1], 2); }
            if (hillflag[0] == 2 && getVectorDistance(hill22, fish2Location) > 200) hillflag[0] = 1;
            if (hillflag[0] == 2 && allEqual(hillflag, 1, 3, 10))
            {
                hillflag[1] = 1;
                hillflag[0] = 3;
                timeForPoseToPose[2] = 0;
            }
            #endregion
            #region 山字第三阶段
            if (hillflag[1] == 1) Helpers.PoseToPose(ref decisions[1], fish2, hill23, HD23, 40f, 200f, msPerCycle, ref timeForPoseToPose[2]);
            if (getVectorDistance(hill23, fish2Location) < 200)   { hillflag[1] = 2; stopFish(ref decisions[1], 2); }
            if (hillflag[1] == 2 && getVectorDistance(hill23, fish2Location) > 200) hillflag[1] = 1;
            if (hillflag[1] == 2 && allEqual(hillflag, 1, 3, 10))
                hillflag[1] = 3;
            #endregion
            #region 定住5s，进入下一函数
            if (hillflag[1] == 3)
            {
                timeflag++;
                if (timeflag >= 50)
                {
                    for (int i = 0; i < 11; i++)
                        timeForPoseToPose[i] = 0;
                    flag++;
                    timeflag = 0;
                }
            }
            #endregion
        }
        #endregion
        #region 数字1
        public static void numberOne(ref Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
            RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
            RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
            RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
            RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
            RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成数字1的目标点
            xna.Vector3 one2 = new xna.Vector3(1614, 0, -420);
            xna.Vector3 one3 = new xna.Vector3(-138, 0, -726);
            xna.Vector3 one4 = new xna.Vector3(108, 0, -702);
            xna.Vector3 one5 = new xna.Vector3(108, 0, -156);
            xna.Vector3 one6 = new xna.Vector3(108, 0, 534);
            xna.Vector3 one7 = new xna.Vector3(-390, 0, 990);
            xna.Vector3 one8 = new xna.Vector3(216, 0, 996);
            xna.Vector3 one9 = new xna.Vector3(810, 0, 984);
            xna.Vector3 one10 = new xna.Vector3(1182, 0, -432);
            #endregion
            #region 构成数字1的目标角度
            float OD2 = (float)-2.0944;
            float OD3 = (float)-1.0472;
            float OD4 = (float)-1.5708;
            float OD5 = (float)-1.5708;
            float OD6 = (float)-1.5708;
            float OD7 = (float)0;
            float OD8 = (float)0;
            float OD9 = (float)0;
            float OD10 = (float)-1.0472;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            //xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            #region 一堆鱼使用PoseToPose函数游到指定位置
            if (oneflag[2] == 0) Helpers.PoseToPose(ref decisions[1], fish2, one2, OD2, 40f, 200f, msPerCycle, ref timeForPoseToPose[2]);
            if (oneflag[3] == 0) Helpers.PoseToPose(ref decisions[2], fish3, one3, OD3, 40f, 200f, msPerCycle, ref timeForPoseToPose[3]);
            if (oneflag[4] == 0) Helpers.PoseToPose(ref decisions[3], fish4, one4, OD4, 40f, 200f, msPerCycle, ref timeForPoseToPose[4]);
            if (oneflag[5] == 0) Helpers.PoseToPose(ref decisions[4], fish5, one5, OD5, 40f, 200f, msPerCycle, ref timeForPoseToPose[5]);
            if (oneflag[6] == 0) Helpers.PoseToPose(ref decisions[5], fish6, one6, OD6, 40f, 200f, msPerCycle, ref timeForPoseToPose[6]);
            if (oneflag[7] == 0) Helpers.PoseToPose(ref decisions[6], fish7, one7, OD7, 40f, 200f, msPerCycle, ref timeForPoseToPose[7]);
            if (oneflag[8] == 0) Helpers.PoseToPose(ref decisions[7], fish8, one8, OD8, 40f, 200f, msPerCycle, ref timeForPoseToPose[8]);
            if (oneflag[9] == 0) Helpers.PoseToPose(ref decisions[8], fish9, one9, OD9, 40f, 200f, msPerCycle, ref timeForPoseToPose[9]);
            if (oneflag[10] == 0) Helpers.PoseToPose(ref decisions[9], fish10, one10, OD10, 40f, 200f, msPerCycle, ref timeForPoseToPose[10]);
            #endregion;
            #region 判断是否到达目标点
            if (getVectorDistance(one2, fish2Location) < 200 && isDirectionRight(OD2, fish2Direction) == 0) { oneflag[2] = 1; stopFish(ref decisions[1], 2); }
            if (getVectorDistance(one3, fish3Location) < 200 && isDirectionRight(OD3, fish3Direction) == 0) { oneflag[3] = 1; stopFish(ref decisions[2], 3); }
            if (getVectorDistance(one4, fish4Location) < 200 && isDirectionRight(OD4, fish4Direction) == 0) { oneflag[4] = 1; stopFish(ref decisions[3], 4); }
            if (getVectorDistance(one5, fish5Location) < 200 && isDirectionRight(OD5, fish5Direction) == 0) { oneflag[5] = 1; stopFish(ref decisions[4], 5); }
            if (getVectorDistance(one6, fish6Location) < 200 && isDirectionRight(OD6, fish6Direction) == 0) { oneflag[6] = 1; stopFish(ref decisions[5], 6); }
            if (getVectorDistance(one7, fish7Location) < 200 && isDirectionRight(OD7, fish7Direction) == 0) { oneflag[7] = 1; stopFish(ref decisions[6], 7); }
            if (getVectorDistance(one8, fish8Location) < 200 && isDirectionRight(OD8, fish8Direction) == 0) { oneflag[8] = 1; stopFish(ref decisions[7], 8); }
            if (getVectorDistance(one9, fish9Location) < 200 && isDirectionRight(OD9, fish9Direction) == 0) { oneflag[9] = 1; stopFish(ref decisions[8], 9); }
            if (getVectorDistance(one10, fish10Location) < 200 && isDirectionRight(OD10, fish10Direction) == 0) { oneflag[10] = 1; stopFish(ref decisions[9], 10); }
            #endregion
            #region 如果达到目标点后被破坏需要修正
            if (getVectorDistance(one2, fish2Location) > 200) oneflag[2] = 0;
            if (getVectorDistance(one3, fish3Location) > 200) oneflag[3] = 0;
            if (getVectorDistance(one4, fish4Location) > 200) oneflag[4] = 0;
            if (getVectorDistance(one5, fish5Location) > 200) oneflag[5] = 0;
            if (getVectorDistance(one6, fish6Location) > 200) oneflag[6] = 0;
            if (getVectorDistance(one7, fish7Location) > 200) oneflag[7] = 0;
            if (getVectorDistance(one8, fish8Location) > 200) oneflag[8] = 0;
            if (getVectorDistance(one9, fish9Location) > 200) oneflag[9] = 0;
            if (getVectorDistance(one10, fish10Location) > 200) oneflag[10] = 0;
            #endregion
            #region 定住5s，进入下一函数
            if (allEqual(oneflag,1,2,10))
            {
                complete = true;
            }
            if(complete)
            {
                timeflag++;
                if (timeflag >= 50)
                {
                    for (int i = 0; i < 11; i++)
                        timeForPoseToPose[i] = 0;
                    flag++;
                    complete = false;
                    timeflag = 0;
                }
            }
            #endregion

        }
        #endregion
        #region 动态圆
        public static void movingCircle(ref Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
            RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
            RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
            RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
            RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
            RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成动态圆的目标点
            xna.Vector3 circle3 = new xna.Vector3(684, 0, -628);
            xna.Vector3 circle4 = new xna.Vector3(-30, 0, -1116);
            xna.Vector3 circle5 = new xna.Vector3(-756, 0, -804);
            xna.Vector3 circle6 = new xna.Vector3(-1008, 0, -192);
            xna.Vector3 circle7 = new xna.Vector3(-708, 0, 558);
            xna.Vector3 circle8 = new xna.Vector3(-60, 0, 864);
            xna.Vector3 circle9 = new xna.Vector3(630, 0, 546);
            xna.Vector3 circle10 = new xna.Vector3(888, 0, -102);
            #endregion
            #region 构成动态圆的目标角度
            float CD3 = (float)0.7854;
            float CD4 = (float)-0.0873;
            float CD5 = (float)-0.7854;
            float CD6 = (float)-1.7453;
            float CD7 = (float)-2.3562;
            float CD8 = (float)2.9671;
            float CD9 = (float)2.3562;
            float CD10 = (float)1.6581;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            //xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            if(complete==false)//未到达指定点
            {
                #region 一堆鱼使用PoseToPose函数游到指定位置
                if (circleflag[3] == 0) Helpers.PoseToPose(ref decisions[2], fish3, circle3, CD3, 40f, 200f, msPerCycle, ref timeForPoseToPose[3]);
                if (circleflag[4] == 0) Helpers.PoseToPose(ref decisions[3], fish4, circle4, CD4, 40f, 200f, msPerCycle, ref timeForPoseToPose[4]);
                if (circleflag[5] == 0) Helpers.PoseToPose(ref decisions[4], fish5, circle5, CD5, 40f, 200f, msPerCycle, ref timeForPoseToPose[5]);
                if (circleflag[6] == 0) Helpers.PoseToPose(ref decisions[5], fish6, circle6, CD6, 40f, 200f, msPerCycle, ref timeForPoseToPose[6]);
                if (circleflag[7] == 0) Helpers.PoseToPose(ref decisions[6], fish7, circle7, CD7, 40f, 200f, msPerCycle, ref timeForPoseToPose[7]);
                if (circleflag[8] == 0) Helpers.PoseToPose(ref decisions[7], fish8, circle8, CD8, 40f, 200f, msPerCycle, ref timeForPoseToPose[8]);
                if (circleflag[9] == 0) Helpers.PoseToPose(ref decisions[8], fish9, circle9, CD9, 40f, 200f, msPerCycle, ref timeForPoseToPose[9]);
                if (circleflag[10] == 0) Helpers.PoseToPose(ref decisions[9], fish10, circle10, CD10, 40f, 200f, msPerCycle, ref timeForPoseToPose[10]);
                #endregion;
                #region 判断是否到达目标点
                if (getVectorDistance(circle3, fish3Location) < 200 && isDirectionRight(CD3, fish3Direction) == 0) { circleflag[3] = 1; stopFish(ref decisions[2], 3); }
                if (getVectorDistance(circle4, fish4Location) < 200 && isDirectionRight(CD4, fish4Direction) == 0) { circleflag[4] = 1; stopFish(ref decisions[3], 4); }
                if (getVectorDistance(circle5, fish5Location) < 200 && isDirectionRight(CD5, fish5Direction) == 0) { circleflag[5] = 1; stopFish(ref decisions[4], 5); }
                if (getVectorDistance(circle6, fish6Location) < 200 && isDirectionRight(CD6, fish6Direction) == 0) { circleflag[6] = 1; stopFish(ref decisions[5], 6); }
                if (getVectorDistance(circle7, fish7Location) < 200 && isDirectionRight(CD7, fish7Direction) == 0) { circleflag[7] = 1; stopFish(ref decisions[6], 7); }
                if (getVectorDistance(circle8, fish8Location) < 200 && isDirectionRight(CD8, fish8Direction) == 0) { circleflag[8] = 1; stopFish(ref decisions[7], 8); }
                if (getVectorDistance(circle9, fish9Location) < 200 && isDirectionRight(CD9, fish9Direction) == 0) { circleflag[9] = 1; stopFish(ref decisions[8], 9); }
                if (getVectorDistance(circle10, fish10Location) < 200 && isDirectionRight(CD10, fish10Direction) == 0) { circleflag[10] = 1; stopFish(ref decisions[9], 10); }
                #endregion
                #region 如果达到目标点后被破坏需要修正
                if (getVectorDistance(circle3, fish3Location) > 200) circleflag[3] = 0;
                if (getVectorDistance(circle4, fish4Location) > 200) circleflag[4] = 0;
                if (getVectorDistance(circle5, fish5Location) > 200) circleflag[5] = 0;
                if (getVectorDistance(circle6, fish6Location) > 200) circleflag[6] = 0;
                if (getVectorDistance(circle7, fish7Location) > 200) circleflag[7] = 0;
                if (getVectorDistance(circle8, fish8Location) > 200) circleflag[8] = 0;
                if (getVectorDistance(circle9, fish9Location) > 200) circleflag[9] = 0;
                if (getVectorDistance(circle10, fish10Location) > 200) circleflag[10] = 0;
                #endregion
            }

            #region 动态圆旋转5s
            if (allEqual(circleflag, 1, 3, 10))
            {
                complete = true;
            }
            if (complete)
            {
                timeflag++;
                for(int i=2;i<10;i++)
                {
                    decisions[i].VCode = 3;
                    decisions[i].TCode = 8;
                }
                if (timeflag >= 80)
                {
                    for (int i = 0; i < 11; i++)
                        timeForPoseToPose[i] = 0;
                    flag++;
                    complete = false;
                    timeForPoseToPose[0] = 1;
                    timeflag = 0;
                }
            }
            #endregion

        }
        #endregion
        #region 与黄鱼互动
        public static void playWithYellowFish(ref Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 声明变量
            int msPerCycle = mission.CommonPara.MsPerCycle;//仿真周期毫秒数
            #region 一堆鱼
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            RoboFish fish5 = mission.TeamsRef[teamId].Fishes[4];
            RoboFish fish6 = mission.TeamsRef[teamId].Fishes[5];
            RoboFish fish7 = mission.TeamsRef[teamId].Fishes[6];
            RoboFish fish8 = mission.TeamsRef[teamId].Fishes[7];
            RoboFish fish9 = mission.TeamsRef[teamId].Fishes[8];
            RoboFish fish10 = mission.TeamsRef[teamId].Fishes[9];
            #endregion
            #endregion
            #region 构成与黄鱼互动的目标点
            xna.Vector3 play3 = new xna.Vector3(-1500, 0, -900);
            xna.Vector3 play4 = new xna.Vector3(-1500, 0, -300);
            xna.Vector3 play5 = new xna.Vector3(-1500, 0, 300);
            xna.Vector3 play6 = new xna.Vector3(-1500, 0, 900);
            xna.Vector3 play7 = new xna.Vector3(1500, 0, -900);
            xna.Vector3 play8 = new xna.Vector3(1500, 0, -300);
            xna.Vector3 play9 = new xna.Vector3(1500, 0, 300);
            xna.Vector3 play10 = new xna.Vector3(1500, 0, 900);
            #endregion
            #region 构成与黄鱼互动的目标角度
            float PD3 = (float)-0.5236;
            float PD4 = (float)-0.5236;
            float PD5 = (float)-0.5236;
            float PD6 = (float)-0.5236;
            float PD7 = (float)2.618;
            float PD8 = (float)2.618;
            float PD9 = (float)2.618;
            float PD10 = (float)2.618;
            #endregion
            #region 获取鱼的位置
            xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
            //xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 fish3Location = mission.TeamsRef[teamId].Fishes[2].PositionMm;
            xna.Vector3 fish4Location = mission.TeamsRef[teamId].Fishes[3].PositionMm;
            xna.Vector3 fish5Location = mission.TeamsRef[teamId].Fishes[4].PositionMm;
            xna.Vector3 fish6Location = mission.TeamsRef[teamId].Fishes[5].PositionMm;
            xna.Vector3 fish7Location = mission.TeamsRef[teamId].Fishes[6].PositionMm;
            xna.Vector3 fish8Location = mission.TeamsRef[teamId].Fishes[7].PositionMm;
            xna.Vector3 fish9Location = mission.TeamsRef[teamId].Fishes[8].PositionMm;
            xna.Vector3 fish10Location = mission.TeamsRef[teamId].Fishes[9].PositionMm;
            #endregion
            #region 获取鱼的角度
            float fish1Direction = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            float fish2Direction = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
            float fish3Direction = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
            float fish4Direction = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
            float fish5Direction = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
            float fish6Direction = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
            float fish7Direction = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
            float fish8Direction = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
            float fish9Direction = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
            float fish10Direction = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
            #endregion
            #region 一堆鱼使用PoseToPose函数游到指定位置
            if (playflag[3] == 0) Helpers.PoseToPose(ref decisions[2], fish3, play3, PD3, 40f, 200f, msPerCycle, ref timeForPoseToPose[3]);
            if (playflag[4] == 0) Helpers.PoseToPose(ref decisions[3], fish4, play4, PD4, 40f, 200f, msPerCycle, ref timeForPoseToPose[4]);
            if (playflag[5] == 0) Helpers.PoseToPose(ref decisions[4], fish5, play5, PD5, 40f, 200f, msPerCycle, ref timeForPoseToPose[5]);
            if (playflag[6] == 0) Helpers.PoseToPose(ref decisions[5], fish6, play6, PD6, 40f, 200f, msPerCycle, ref timeForPoseToPose[6]);
            if (playflag[7] == 0) Helpers.PoseToPose(ref decisions[6], fish7, play7, PD7, 40f, 200f, msPerCycle, ref timeForPoseToPose[7]);
            if (playflag[8] == 0) Helpers.PoseToPose(ref decisions[7], fish8, play8, PD8, 40f, 200f, msPerCycle, ref timeForPoseToPose[8]);
            if (playflag[9] == 0) Helpers.PoseToPose(ref decisions[8], fish9, play9, PD9, 40f, 200f, msPerCycle, ref timeForPoseToPose[9]);
            if (playflag[10] == 0) Helpers.PoseToPose(ref decisions[9], fish10, play10, PD10, 40f, 200f, msPerCycle, ref timeForPoseToPose[10]);
            #endregion;
            #region 判断是否到达目标点
            if (getVectorDistance(play3, fish3Location) < 200 && isDirectionRight(PD3, fish3Direction) == 0) { playflag[3] = 1; stopFish(ref decisions[2], 3); }
            if (getVectorDistance(play4, fish4Location) < 200 && isDirectionRight(PD4, fish4Direction) == 0) { playflag[4] = 1; stopFish(ref decisions[3], 4); }
            if (getVectorDistance(play5, fish5Location) < 200 && isDirectionRight(PD5, fish5Direction) == 0) { playflag[5] = 1; stopFish(ref decisions[4], 5); }
            if (getVectorDistance(play6, fish6Location) < 200 && isDirectionRight(PD6, fish6Direction) == 0) { playflag[6] = 1; stopFish(ref decisions[5], 6); }
            if (getVectorDistance(play7, fish7Location) < 200 && isDirectionRight(PD7, fish7Direction) == 0) { playflag[7] = 1; stopFish(ref decisions[6], 7); }
            if (getVectorDistance(play8, fish8Location) < 200 && isDirectionRight(PD8, fish8Direction) == 0) { playflag[8] = 1; stopFish(ref decisions[7], 8); }
            if (getVectorDistance(play9, fish9Location) < 200 && isDirectionRight(PD9, fish9Direction) == 0) { playflag[9] = 1; stopFish(ref decisions[8], 9); }
            if (getVectorDistance(play10, fish10Location) < 200 && isDirectionRight(PD10, fish10Direction) == 0) { playflag[10] = 1; stopFish(ref decisions[9], 10); }
            #endregion
            #region 如果达到目标点后被破坏需要修正
            if (getVectorDistance(play3, fish3Location) > 200) playflag[3] = 0;
            if (getVectorDistance(play4, fish4Location) > 200) playflag[4] = 0;
            if (getVectorDistance(play5, fish5Location) > 200) playflag[5] = 0;
            if (getVectorDistance(play6, fish6Location) > 200) playflag[6] = 0;
            if (getVectorDistance(play7, fish7Location) > 200) playflag[7] = 0;
            if (getVectorDistance(play8, fish8Location) > 200) playflag[8] = 0;
            if (getVectorDistance(play9, fish9Location) > 200) playflag[9] = 0;
            if (getVectorDistance(play10, fish10Location) > 200) playflag[10] = 0;
            #endregion
            #region 定住2s，进入下一函数
            if (allEqual(playflag, 1, 3, 10))
            {
                complete = true;
            }
            if (complete)
            {
                timeflag++;
                if (timeflag >= 20)
                {
                    for (int i = 0; i < 11; i++)
                        timeForPoseToPose[i] = 0;
                    timeflag = 0;
                    flag++;
                    complete = false;
                }
            }
            #endregion
        }
        #endregion
        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
                preDecisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }
            mission.CommonPara.MsPerCycle = 100;
            #region 决策计算过程 需要各参赛队伍实现的部分
            #region 策略编写帮助信息
            //====================我是华丽的分割线====================//
            //======================策略编写指南======================//
            //1.策略编写工作直接目标是给当前队伍决策数组decisions各元素填充决策值
            //2.决策数据类型包括两个int成员，VCode为速度档位值，TCode为转弯档位值
            //3.VCode取值范围0-14共15个整数值，每个整数对应一个速度值，速度值整体但非严格递增
            //有个别档位值对应的速度值低于比它小的档位值对应的速度值，速度值数据来源于实验
            //4.TCode取值范围0-14共15个整数值，每个整数对应一个角速度值
            //整数7对应直游，角速度值为0，整数6-0，8-14分别对应左转和右转，偏离7越远，角度速度值越大
            //5.任意两个速度/转弯档位之间切换，都需要若干个仿真周期，才能达到稳态速度/角速度值
            //目前运动学计算过程决定稳态速度/角速度值接近但小于目标档位对应的速度/角速度值
            //6.决策类Strategy的实例在加载完毕后一直存在于内存中，可以自定义私有成员变量保存必要信息
            //但需要注意的是，保存的信息在中途更换策略时将会丢失
            //====================我是华丽的分割线====================//
            //=======策略中可以使用的比赛环境信息和过程信息说明=======//
            //场地坐标系: 以毫米为单位，矩形场地中心为原点，向右为正X，向下为正Z
            //            负X轴顺时针转回负X轴角度范围为(-PI,PI)的坐标系，也称为世界坐标系
            //mission.CommonPara: 当前仿真使命公共参数
            //mission.CommonPara.FishCntPerTeam: 每支队伍仿真机器鱼数量
            //mission.CommonPara.MsPerCycle: 仿真周期毫秒数
            //mission.CommonPara.RemainingCycles: 当前剩余仿真周期数
            //mission.CommonPara.TeamCount: 当前仿真使命参与队伍数量
            //mission.CommonPara.TotalSeconds: 当前仿真使命运行时间秒数
            //mission.EnvRef.Balls: 
            //当前仿真使命涉及到的仿真水球列表，列表元素的成员意义参见URWPGSim2D.Common.Ball类定义中的注释
            //mission.EnvRef.FieldInfo: 
            //当前仿真使命涉及到的仿真场地，各成员意义参见URWPGSim2D.Common.Field类定义中的注释
            //mission.EnvRef.ObstaclesRect: 
            //当前仿真使命涉及到的方形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RectangularObstacle类定义中的注释
            //mission.EnvRef.ObstaclesRound:
            //当前仿真使命涉及到的圆形障碍物列表，列表元素的成员意义参见URWPGSim2D.Common.RoundedObstacle类定义中的注释
            //mission.TeamsRef[teamId]:
            //决策类当前对象对应的仿真使命参与队伍（当前队伍）
            //mission.TeamsRef[teamId].Para:
            //当前队伍公共参数，各成员意义参见URWPGSim2D.Common.TeamCommonPara类定义中的注释
            //mission.TeamsRef[teamId].Fishes:
            //当前队伍仿真机器鱼列表，列表元素的成员意义参见URWPGSim2D.Common.RoboFish类定义中的注释
            //mission.TeamsRef[teamId].Fishes[i].PositionMm和PolygonVertices[0],BodyDirectionRad,VelocityMmPs,
            //                                   AngularVelocityRadPs,Tactic:
            //当前队伍第i条仿真机器鱼鱼体矩形中心和鱼头顶点在场地坐标系中的位置（用到X坐标和Z坐标），鱼体方向，速度值，
            //                                   角速度值，决策值
            //====================我是华丽的分割线====================//
            //========================典型循环========================//
            //for (int i = 0; i < mission.CommonPara.FishCntPerTeam; i++)
            //{
            //  decisions[i].VCode = 0; // 静止
            //  decisions[i].TCode = 7; // 直游
            //}
            //====================我是华丽的分割线====================//
            #endregion

            #endregion
            
            //xna.Vector3 hill9 = new xna.Vector3(834, 0, 996);
            //float HD9 = 0;
            //if (getVectorDistance(hill9, mission.TeamsRef[teamId].Fishes[8].PositionMm) < 200 && isDirectionRight(HD9, mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad) == 0) { hillflag[9] = 1; stopFish(ref decisions[8]); }
            //if (getVectorDistance(hill9, mission.TeamsRef[teamId].Fishes[8].PositionMm) > 200) hillflag[9] = 0;
            //if (hillflag[9] == 0)
            //    Helpers.PoseToPose(ref decisions[8], mission.TeamsRef[teamId].Fishes[8], hill9, HD9, 45f, 200f, 100, ref timeForPoseToPose[9]);

            if (flag == 0) 
                hillCharacter(ref mission, teamId, ref decisions);

            if (flag == 1) 
                numberOne(ref mission, teamId, ref decisions);

            if (flag == 2)
                playWithYellowFish(ref mission, teamId, ref decisions);

            if (flag == 3)
                movingCircle(ref mission, teamId, ref decisions);

            if (flag == 4) 
            {
                for(int i=1;i<10;i++)
                {
                    stopFish(ref decisions[i], i + 1);
                }
            }
            /*
            xna.Vector3 fish1Location2 = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[2];
            xna.Vector3 fish2Location = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            xna.Vector3 A1 = new xna.Vector3(-12, 0, -696);
            float AD1 = (float)-2.1991;
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            //Helpers.fishMoving(A1, AD1, ref decisions[1], ref fish2, ref timeflag);
            if (zeroflag[1] == 0) Helpers.Dribble(ref decisions[1], fish2, A1, AD1, 30, 10, 60, 14, 5, 18, mission.CommonPara.MsPerCycle, true);
            Helpers.Dribble(ref decisions[9], mission.TeamsRef[teamId].Fishes[9], fish1Location2, (float)2.0769, 30, 30, 30, 14, 12, 5, mission.CommonPara.MsPerCycle, true);
            if (isDirectionRight(mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad, AD1) == 0 && getVectorDistance(A1, fish2Location) < 45) { decisions[1].VCode = 0; zeroflag[1] = 1; decisions[1].TCode = 7; }
            if (getVectorDistance(A1, fish2Location) > 60) zeroflag[1] = 0;
            */
            return decisions;
        }
    }
}


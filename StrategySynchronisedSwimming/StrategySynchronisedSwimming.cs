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
        public static int isDirectionRight(float a, float b)
        {
            while (a > Math.PI) a -= (float)(2 * Math.PI);
            while (b > Math.PI) b -= (float)(2 * Math.PI);
            while (a < -Math.PI) a += (float)(2 * Math.PI);
            while (b < -Math.PI) b += (float)(2 * Math.PI);
            if (a - b > 0.12) return 1;//a在b右边
            else if (a - b < -0.12) return -1; //a在b左边
            else return 0;
        }
        public static float getVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2) + Math.Pow((a.Z - b.Z), 2)));
        }
        Decision[] preDecisions = null;     
        private int flag = 0;//主函数标志值
        private int timeflag = 0;
        private int remainRecord = 0;
        private int[] zeroflag = new int[10];
        private int[] flyflag = new int[10];
        
        /// <summary>
        /// 获取当前仿真使命（比赛项目）当前队伍所有仿真机器鱼的决策数据构成的数组
        /// </summary>
        /// <param name="mission">服务端当前运行着的仿真使命Mission对象</param>
        /// <param name="teamId">当前队伍在服务端运行着的仿真使命中所处的编号 
        /// 用于作为索引访问Mission对象的TeamsRef队伍列表中代表当前队伍的元素</param>
        /// <returns>当前队伍所有仿真机器鱼的决策数据构成的Decision数组对象</returns>
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
          */
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
            if (zeroflag[1] == 0) Helpers.Dribble(ref decisions[1], fish2, A1, AD1, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[2] == 0) Helpers.Dribble(ref decisions[2], fish3, A2, AD2, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[3] == 0) Helpers.Dribble(ref decisions[3], fish4, A3, AD3, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[4] == 0) Helpers.Dribble(ref decisions[4], fish5, A4, AD4, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[5] == 0) Helpers.Dribble(ref decisions[5], fish6, A5, AD5, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[6] == 0) Helpers.Dribble(ref decisions[6], fish7, A6, AD6, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[7] == 0) Helpers.Dribble(ref decisions[7], fish8, A7, AD7, 10, 6, 45, 14, 2, 18, msPerCycle, true);
            if (zeroflag[8] == 0) Helpers.Dribble(ref decisions[8], fish9, A8, AD8, 10, 6, 45, 14, 2, 18, msPerCycle, true);

            Helpers.Dribble(ref decisions[9], fish10, fish1Location2, (float)2.0769, 30, 20, 0, 14, 12, 5, msPerCycle, true);
            #endregion;
            #region 判断全部鱼在指定位置,完成后flag=1
            if (isDirectionRight(fish2Direction, AD1) == 0 && getVectorDistance(A1, fish2Location) < 45) { decisions[1].VCode = 0; zeroflag[1] = 1; decisions[1].TCode = 7; }
            if (isDirectionRight(fish3Direction, AD2) == 0 && getVectorDistance(A2, fish3Location) < 45) { decisions[2].VCode = 0; zeroflag[2] = 1; decisions[2].TCode = 7; }
            if (isDirectionRight(fish4Direction, AD3) == 0 && getVectorDistance(A3, fish4Location) < 45) { decisions[3].VCode = 0; zeroflag[3] = 1; decisions[3].TCode = 7; }
            if (isDirectionRight(fish5Direction, AD4) == 0 && getVectorDistance(A4, fish5Location) < 45) { decisions[4].VCode = 0; zeroflag[4] = 1; decisions[4].TCode = 7; }
            if (isDirectionRight(fish6Direction, AD5) == 0 && getVectorDistance(A5, fish6Location) < 45) { decisions[5].VCode = 0; zeroflag[5] = 1; decisions[5].TCode = 7; }
            if (isDirectionRight(fish7Direction, AD6) == 0 && getVectorDistance(A6, fish7Location) < 45) { decisions[6].VCode = 0; zeroflag[6] = 1; decisions[6].TCode = 7; }
            if (isDirectionRight(fish8Direction, AD7) == 0 && getVectorDistance(A7, fish8Location) < 45) { decisions[7].VCode = 0; zeroflag[7] = 1; decisions[7].TCode = 7; }
            if (isDirectionRight(fish9Direction, AD8) == 0 && getVectorDistance(A8, fish9Location) < 45) { decisions[8].VCode = 0; zeroflag[8] = 1; decisions[8].TCode = 7; }

            if (getVectorDistance(A1, fish2Location) > 60) zeroflag[1] = 0;
            if (getVectorDistance(A2, fish3Location) > 60) zeroflag[2] = 0;
            if (getVectorDistance(A3, fish4Location) > 60) zeroflag[3] = 0;
            if (getVectorDistance(A4, fish5Location) > 60) zeroflag[4] = 0;
            if (getVectorDistance(A5, fish6Location) > 60) zeroflag[5] = 0;
            if (getVectorDistance(A6, fish7Location) > 60) zeroflag[6] = 0;
            if (getVectorDistance(A7, fish8Location) > 60) zeroflag[7] = 0;
            if (getVectorDistance(A8, fish9Location) > 60) zeroflag[8] = 0;


            if (timeflag != 1 && zeroflag[1] == 1 && zeroflag[2] == 1 && zeroflag[3] == 1 && zeroflag[4] == 1 && zeroflag[5] == 1 && zeroflag[6] == 1 && zeroflag[7] == 1 && zeroflag[8] == 1) 
                timeflag = 1;
           
            if (timeflag==1)
            {
                remainRecord = mission.CommonPara.RemainingCycles;
                timeflag = 2;
            }
            /*if(timeflag==1||timeflag==2)
            {
                StreamWriter log = new StreamWriter("C:\\Users\\wujun\\Desktop\\URWPGSim2D\\URWPGSim2D\\Strategy\\log.txt", true);
                log.WriteLine(remainRecord);
                log.WriteLine(mission.CommonPara.RemainingCycles);
                log.Close();
            }*/
            if(timeflag==2)
            {
                if(remainRecord-mission.CommonPara.RemainingCycles>=30)
                {
                    timeflag = 0;
                    flag = 1;
                }
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
            //请从这里开始编写代码
            /*
             #region 获取鱼的位置
             xna.Vector3 fish1Location = mission.TeamsRef[teamId].Fishes[0].PositionMm;
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
             float fish1Rad = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
             float fish2Rad = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;
             float fish3Rad = mission.TeamsRef[teamId].Fishes[2].BodyDirectionRad;
             float fish4Rad = mission.TeamsRef[teamId].Fishes[3].BodyDirectionRad;
             float fish5Rad = mission.TeamsRef[teamId].Fishes[4].BodyDirectionRad;
             float fish6Rad = mission.TeamsRef[teamId].Fishes[5].BodyDirectionRad;
             float fish7Rad = mission.TeamsRef[teamId].Fishes[6].BodyDirectionRad;
             float fish8Rad = mission.TeamsRef[teamId].Fishes[7].BodyDirectionRad;
             float fish9Rad = mission.TeamsRef[teamId].Fishes[8].BodyDirectionRad;
             float fish10Rad = mission.TeamsRef[teamId].Fishes[9].BodyDirectionRad;
             #endregion

             #region 获取鱼的速度
             float fish1Speed = mission.TeamsRef[teamId].Fishes[0].VelocityMmPs;
             float fish2Speed = mission.TeamsRef[teamId].Fishes[1].VelocityMmPs;
             float fish3Speed = mission.TeamsRef[teamId].Fishes[2].VelocityMmPs;
             float fish4Speed = mission.TeamsRef[teamId].Fishes[3].VelocityMmPs;
             float fish5Speed = mission.TeamsRef[teamId].Fishes[4].VelocityMmPs;
             float fish6Speed = mission.TeamsRef[teamId].Fishes[5].VelocityMmPs;
             float fish7Speed = mission.TeamsRef[teamId].Fishes[6].VelocityMmPs;
             float fish8Speed = mission.TeamsRef[teamId].Fishes[7].VelocityMmPs;
             float fish9Speed = mission.TeamsRef[teamId].Fishes[8].VelocityMmPs;
             float fish10Speed = mission.TeamsRef[teamId].Fishes[9].VelocityMmPs;
             #endregion




        */
            #endregion
            if(flag==0)
                Zero(ref mission, teamId, ref decisions);
            if(flag==1)
                FlyCharacter(ref mission, teamId, ref decisions);
           /* 
            xna.Vector3 A1 = new xna.Vector3(-12, 0, -696);
            float AD1 = (float)-2.1991;
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            Helpers.fishMoving(A1, AD1, ref decisions[1], ref fish2, ref timeflag);
            */

            return decisions;
        }
    }
}


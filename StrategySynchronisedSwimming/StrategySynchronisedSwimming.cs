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
            if (a - b > 0.15) return 1;//a在b右边
            else if (a - b < -0.15) return -1; //a在b左边
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
        public static void fishToPoint(ref Decision decisions, RoboFish fish, xna.Vector3 targetePoint, float targetDirection, int noOfFish, ref int[] timeForPoseToPose, int[] flag)
        {
            switch (flag[noOfFish])
            {
                case 0:
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 100)
                    {
                        Helpers.PoseToPose(ref decisions, fish, targetePoint, targetDirection, 6f, 10f, 100, ref timeForPoseToPose[noOfFish]);
                    }
                  //  Helpers.PoseToPose(ref decisions, fish, targetePoint, targetDirection, 6f, 10f, 50, ref timeForPoseToPose[noOfFish]);
                    if (getVectorDistance(targetePoint, fish.PositionMm) < 100)
                    {
                        flag[noOfFish] = 1;
                    }
                    break;
                case 1:
                    if (isDirectionRight(targetDirection, fish.BodyDirectionRad) < 0)
                    {
                        decisions.TCode = 0;
                        decisions.VCode = 1;
                    }
                    else if (isDirectionRight(targetDirection, fish.BodyDirectionRad) > 0)
                    {
                        decisions.TCode = 14;
                        decisions.VCode = 1;
                    }
                    else
                    {
                        flag[noOfFish] = 2;
                        stopFish(ref decisions, noOfFish);
                    }
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 150)
                        flag[noOfFish] = 0;
                    break;
                case 2:
                    if (isDirectionRight(targetDirection, fish.BodyDirectionRad) < 0)
                    {
                        decisions.TCode = 0;
                        decisions.VCode = 1;
                    }
                    else if (isDirectionRight(targetDirection, fish.BodyDirectionRad) > 0)
                    {
                        decisions.TCode = 14;
                        decisions.VCode = 1;
                    }
                    else
                    {
                        stopFish(ref decisions, noOfFish);
                    }
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 150)
                        flag[noOfFish] = 0;
                    break;
                default:
                    decisions.TCode = 7;
                    decisions.VCode = 0;
                    break;
            }
        }
        public static void dribbleFishToPoint(ref Decision decisions, RoboFish fish, xna.Vector3 targetePoint, float targetDirection, int noOfFish, int[] flag)
        {
            switch (flag[noOfFish])
            {
                case 0:
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 100)
                    {
                        Helpers.Dribble(ref decisions, fish, targetePoint, targetDirection, 20f, 10f, 50f, 14, 10, 15, 100, false);
                    }
                    if (getVectorDistance(targetePoint, fish.PositionMm) < 100)
                    {
                        flag[noOfFish] = 1;
                    }
                    break;
                case 1:
                    if (isDirectionRight(targetDirection, fish.BodyDirectionRad) < 0)
                    {
                        decisions.TCode = 0;
                        decisions.VCode = 1;
                    }
                    else if (isDirectionRight(targetDirection, fish.BodyDirectionRad) > 0)
                    {
                        decisions.TCode = 14;
                        decisions.VCode = 1;
                    }
                    else
                    {
                        flag[noOfFish] = 2;
                        stopFish(ref decisions, noOfFish);
                    }
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 150)
                        flag[noOfFish] = 0;
                    break;
                case 2:
                    if (isDirectionRight(targetDirection, fish.BodyDirectionRad) < 0)
                    {
                        decisions.TCode = 0;
                        decisions.VCode = 1;
                    }
                    else if (isDirectionRight(targetDirection, fish.BodyDirectionRad) > 0)
                    {
                        decisions.TCode = 14;
                        decisions.VCode = 1;
                    }
                    else
                    {
                        stopFish(ref decisions, noOfFish);
                    }
                    if (getVectorDistance(targetePoint, fish.PositionMm) > 150)
                        flag[noOfFish] = 0;
                    break;
                default:
                    decisions.TCode = 7;
                    decisions.VCode = 0;
                    break;

            }
        }
        public static int completeCircle = 0;
        Decision[] preDecisions = null;
        private static int flag = 0;//主函数标志值
        private static int timeflag = 0;
        private static int[] timeForPoseToPose = new int[11];
        private static bool complete = false;
        //private int remainRecord = 0;
        //private int[] zeroflag = new int[10];
        //private int[] flyflag = new int[10];
        private static int[] startRoadflag = new int[11];
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
        #region 开始路
        public static void startRoad(ref Mission mission, int teamId, ref Decision[] decisions)
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
            #region 构成开始路的目标点
            xna.Vector3 startRoad2 = new xna.Vector3(-780, 0, -954);
            xna.Vector3 startRoad3 = new xna.Vector3(-750, 0, 282);
            xna.Vector3 startRoad4 = new xna.Vector3(-60, 0, 834);
            xna.Vector3 startRoad5 = new xna.Vector3(588, 0, -984);
            xna.Vector3 startRoad6 = new xna.Vector3(-882, 0, -420);
            xna.Vector3 startRoad7 = new xna.Vector3(-810, 0, 972);
            xna.Vector3 startRoad8 = new xna.Vector3(492, 0, -402);
            xna.Vector3 startRoad9 = new xna.Vector3(594, 0, 954);
            xna.Vector3 startRoad10 = new xna.Vector3(660, 0, 202);
            #endregion
            #region 构成开始路的目标角度
            float SRD2 = (float)1.0472;
            float SRD3 = (float)1.0472;
            float SRD4 = (float)-1.5708;
            float SRD5 = (float)1.0472;
            float SRD6 = (float)2.0944;
            float SRD7 = (float)2.0944;
            float SRD8 = (float)2.0944;
            float SRD9 = (float)2.0944;
            float SRD10 = (float)1.0472;
            #endregion
            #region 一堆鱼移动到目标点和目标角度
            fishToPoint(ref decisions[1], fish2, startRoad2, SRD2, 2, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[2], fish3, startRoad3, SRD3, 3, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[3], fish4, startRoad4, SRD4, 4, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[4], fish5, startRoad5, SRD5, 5, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[5], fish6, startRoad6, SRD6, 6, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[6], fish7, startRoad7, SRD7, 7, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[7], fish8, startRoad8, SRD8, 8, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[8], fish9, startRoad9, SRD9, 9, ref timeForPoseToPose, startRoadflag);
            fishToPoint(ref decisions[9], fish10, startRoad10, SRD10, 10, ref timeForPoseToPose, startRoadflag);
            #endregion
            #region 定住3s，进入下一函数
            if (allEqual(oneflag, 2, 2, 10))
            {
                complete = true;
            }
            if (complete)
            {
                timeflag++;
                if (timeflag >= 30)
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
            xna.Vector3 hill23 = new xna.Vector3(1644, 0, 300);       
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
            #region 一堆鱼移动到目标点和目标角度
            if(hillflag[0]==0)
                fishToPoint(ref decisions[1], fish2, hill21, HD21, 2, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[2], fish3, hill3, HD3, 3, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[3], fish4, hill4, HD4, 4, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[4], fish5, hill5, HD5, 5, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[5], fish6, hill6, HD6, 6, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[6], fish7, hill7, HD7, 7, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[7], fish8, hill8, HD8, 8, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[8], fish9, hill9, HD9, 9, ref timeForPoseToPose, hillflag);
            fishToPoint(ref decisions[9], fish10, hill10, HD10, 10, ref timeForPoseToPose, hillflag);
            #endregion
            #region 山字第二阶段
            if (hillflag[0] == 0 && allEqual(hillflag, 2, 2, 10))
            {
                hillflag[0] = 1;
                timeForPoseToPose[2] = 0;
            }
            if (hillflag[0] == 1)
                dribbleFishToPoint(ref decisions[1], fish2, hill22, HD22, 2, hillflag);

            if (hillflag[0] == 1 && hillflag[2] == 1) 
            {
                hillflag[1] = 1;
                hillflag[0] = 3;
                timeForPoseToPose[2] = 0;
            }
            #endregion
            #region 山字第三阶段
            if (isDirectionRight(fish2.BodyDirectionRad, HD23) == 0)
            {
                if (hillflag[1] == 1 && allEqual(hillflag, 2, 2, 10))
                {
                    for (int i = 1; i < 11; i++) {
                        hillflag[i] = 3;
                    }
                }
            }

            if (hillflag[1] == 1)
                dribbleFishToPoint(ref decisions[1], fish2, hill23, HD23, 2, hillflag);
            #endregion
            #region 定住5s，进入下一函数
            if (hillflag[1] == 3)
            {
                timeflag++;
                if (timeflag >= 60)
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
            xna.Vector3 one2 = new xna.Vector3(1656, 0, -582);
            xna.Vector3 one3 = new xna.Vector3(-276, 0, -504);
            xna.Vector3 one4 = new xna.Vector3(108, 0, -702);
            xna.Vector3 one5 = new xna.Vector3(108, 0, -156);
            xna.Vector3 one6 = new xna.Vector3(108, 0, 534);
            xna.Vector3 one7 = new xna.Vector3(-390, 0, 996);
            xna.Vector3 one8 = new xna.Vector3(216, 0, 996);
            xna.Vector3 one9 = new xna.Vector3(834, 0, 984);
            xna.Vector3 one10 = new xna.Vector3(1266, 0, -444);
            #endregion
            #region 构成数字1的目标角度
            float OD2 = (float)-0.7854;
            float OD3 = (float)2.0944;
            float OD4 = (float)-1.5708;
            float OD5 = (float)-1.5708;
            float OD6 = (float)-1.5708;
            float OD7 = 0;
            float OD8 = 0;
            float OD9 = 0;
            float OD10 = (float)1.0647;
            #endregion
            #region 一堆鱼移动到目标点和目标角度
            fishToPoint(ref decisions[1], fish2, one2, OD2, 2, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[2], fish3, one3, OD3, 3, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[3], fish4, one4, OD4, 4, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[4], fish5, one5, OD5, 5, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[5], fish6, one6, OD6, 6, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[6], fish7, one7, OD7, 7, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[7], fish8, one8, OD8, 8, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[8], fish9, one9, OD9, 9, ref timeForPoseToPose, oneflag);
            fishToPoint(ref decisions[9], fish10, one10, OD10, 10, ref timeForPoseToPose, oneflag);
            #endregion
            #region 定住5s，进入下一函数
            if (allEqual(oneflag, 2, 2, 10)) 
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
            #region 动态圆旋转
            if (completeCircle == 0) //未到达指定点
            {
                #region 一堆鱼移动到目标点和目标角度
                fishToPoint(ref decisions[2], fish3, circle3, CD3, 3, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[3], fish4, circle4, CD4, 4, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[4], fish5, circle5, CD5, 5, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[5], fish6, circle6, CD6, 6, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[6], fish7, circle7, CD7, 7, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[7], fish8, circle8, CD8, 8, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[8], fish9, circle9, CD9, 9, ref timeForPoseToPose, circleflag);
                fishToPoint(ref decisions[9], fish10, circle10, CD10, 10, ref timeForPoseToPose, circleflag);
                #endregion
                #region 开始旋转
                if (allEqual(circleflag, 2, 3, 10))
                {
                    completeCircle = 1;
                    for (int i = 0; i < 11; i++)//参数清零
                    {
                        circleflag[i] = 0;
                        timeForPoseToPose[i] = 0;
                    }
                }
                #endregion
            }
            
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
            #region 一堆鱼移动到目标点和目标角度
            fishToPoint(ref decisions[2], fish3, play3, PD3, 3, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[3], fish4, play4, PD4, 4, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[4], fish5, play5, PD5, 5, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[5], fish6, play6, PD6, 6, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[6], fish7, play7, PD7, 7, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[7], fish8, play8, PD8, 8, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[8], fish9, play9, PD9, 9, ref timeForPoseToPose, playflag);
            fishToPoint(ref decisions[9], fish10, play10, PD10, 10, ref timeForPoseToPose, playflag);
            #endregion
            #region 定住2s，进入下一函数
            if (allEqual(playflag, 2, 3, 10))
            {
                complete = true;
            }
            if (complete)
            {
                timeflag++;
                if (timeflag >= 30)
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
            //if(flag==5)//test
            //{
            //    xna.Vector3 p1 = new xna.Vector3(-1500, 0, 50);
            //    xna.Vector3 p2 = new xna.Vector3(1500, 0, 50);
            //    float direction = 0;
                
            //    //switch (hillflag[2])
            //    //{
            //    //    case 0:
            //    //        Helpers.PoseToPose(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], p2, direction, 6f, 200f, 100, ref timeForPoseToPose[2]);
            //    //        if (getVectorDistance(p2, mission.TeamsRef[teamId].Fishes[1].PositionMm) < 150)
            //    //        {
            //    //            hillflag[2]++;  
            //    //        }
            //    //        break;
            //    //    case 1:
            //    //        if (isDirectionRight(direction, mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad) < 0)
            //    //        {
            //    //            decisions[1].TCode = 2;
            //    //            decisions[1].VCode = 1;
            //    //        }
            //    //        else if (isDirectionRight(direction, mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad) > 0)
            //    //        {
            //    //            decisions[1].TCode = 12;
            //    //            decisions[1].VCode = 1;
            //    //        }
            //    //        else
            //    //            stopFish(ref decisions[1], 2);

            //    //        if (getVectorDistance(p2, mission.TeamsRef[teamId].Fishes[1].PositionMm) > 150)
            //    //            hillflag[2]--;
            //    //        break;
            //    //}


            //    //        if (hillflag[2] == 0)
            //    //{
            //    //    Helpers.PoseToPose(ref decisions[1], mission.TeamsRef[teamId].Fishes[1], p2, direction, 6f, 200f, 100, ref timeForPoseToPose[2]);

            //    //    if (getVectorDistance(p2, mission.TeamsRef[teamId].Fishes[1].PositionMm) < 150)
            //    //    {
            //    //        hillflag[2] = 1;
            //    //        stopFish(ref decisions[1], 2);
            //    //    }
            //    //    else if (getVectorDistance(p2, mission.TeamsRef[teamId].Fishes[1].PositionMm) > 150)
            //    //        hillflag[2] = 0;

            //    //}


            //    //else if (isDirectionRight(direction, mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad) < 0 && hillflag[2] != 0)
            //    //{
            //    //    decisions[1].TCode = 2;
            //    //    decisions[1].VCode = 1;
            //    //}
            //    //else if (isDirectionRight(direction, mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad) > 0 && hillflag[2] != 0)
            //    //{
            //    //    decisions[1].TCode = 12;
            //    //    decisions[1].VCode = 1;
            //    //}
            //    //else decisions[1].VCode = 0;



            //}

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


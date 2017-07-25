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
        public static int[] timeForPoseToPose = new int[5];
        //public static xna.Vector3 CalCirclePoint(RoboFish fightFish, xna.Vector3 centralPoint)
        //{
        //    int radius = 500;
        //    double destX = centralPoint.X;
        //    double destZ = centralPoint.Z;
        //    xna.Vector3 fishTowardsPoint = centralPoint - fightFish.PositionMm;
        //    double angle = MathHelper.ToRadians(Helpers.GetAngleDegree(fishTowardsPoint));
        //    if (angle >= 0)
        //    {
        //        if (angle > Math.PI / 2) 
        //        {
        //            angle = Math.PI - angle;
        //            destX -= radius * Math.Cos(angle);
        //            destZ += radius * Math.Sin(angle);
        //        }
        //        else
        //        {
        //            destX += radius * Math.Cos(angle);
        //            destZ += radius * Math.Sin(angle);
        //        }
        //    }
        //    else
        //    {
        //        if (angle < -Math.PI / 2) 
        //        {
        //            angle = Math.PI - Math.Abs(angle);
        //            destX -= radius * Math.Cos(angle);
        //            destZ -= radius * Math.Sin(angle);
        //        }
        //        else
        //        {
        //            angle = Math.Abs(angle);
        //            destX += radius * Math.Sin(angle);
        //            destZ -= radius * Math.Cos(angle);
        //        }
        //    }
        //    xna.Vector3 destPoint = new xna.Vector3((float)destX, 0, (float)destZ);
        //    //StreamWriter log = new StreamWriter("C:\\Users\\wujun\\Desktop\\URWPGSim2D\\URWPGSim2D\\Strategy\\log.txt", true);
        //    //log.Write(destPoint.X);
        //    //log.Write(' ');
        //    //log.Write(destPoint.Z);
        //    //log.WriteLine("end");
        //    //log.Close();
        //    return destPoint;
        //}
        public static float CorrectRad(float angleToCorrect)
        {
            if (angleToCorrect > Math.PI)
                angleToCorrect -= 2 * (float)Math.PI;
            if (angleToCorrect < -Math.PI) 
                angleToCorrect += 2 * (float)Math.PI;
            return angleToCorrect;
        }
        public static void JudgeFish(ref Decision[] decisions,RoboFish fish,xna.Vector3 destPosition, int i)
        {
            if(GetVectorDistance(fish.PositionMm,destPosition)>150)
            {
                timeForPoseToPose[i] = 0;
                decisions[i - 1].VCode = 0;
                
            }
                
        }
        public static float GetVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2d) + Math.Pow((a.Z - b.Z), 2d)));
        }
        /// <summary>
        /// 获取当前仿真使命（比赛项目）当前队伍所有仿真机器鱼的决策数据构成的数组
        /// </summary>
        /// <param name="mission">服务端当前运行着的仿真使命Mission对象</param>
        /// <param name="teamId">当前队伍在服务端运行着的仿真使命中所处的编号 
        /// 用于作为索引访问Mission对象的TeamsRef队伍列表中代表当前队伍的元素</param>
        /// <returns>当前队伍所有仿真机器鱼的决策数据构成的Decision数组对象</returns>
        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }
            mission.CommonPara.MsPerCycle = 100;
            #region 检查鱼是否被抓
            //int by2 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish2Caught"]);
            //int by3 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish3Caught"]);
            //int by4 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish4Caught"]);
            //int br2 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish2Caught"]);
            //int br3 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish3Caught"]);
            //int br4 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish4Caught"]);
            #endregion
            #region 一堆鱼
            RoboFish fightFish = mission.TeamsRef[(teamId + 1) % 2].Fishes[0];
            RoboFish protectFish = mission.TeamsRef[teamId].Fishes[0];
            RoboFish fish2 = mission.TeamsRef[teamId].Fishes[1];
            RoboFish fish3 = mission.TeamsRef[teamId].Fishes[2];
            RoboFish fish4 = mission.TeamsRef[teamId].Fishes[3];
            #endregion
            #region 障碍物中心点
            xna.Vector3 blockUp = mission.EnvRef.ObstaclesRect[0].PositionMm;
            xna.Vector3 blockMiddle = mission.EnvRef.ObstaclesRect[1].PositionMm;
            xna.Vector3 blockDown = mission.EnvRef.ObstaclesRect[2].PositionMm;
            #endregion
            //#region 圆形算法躲避
            //JudgeFish(ref decisions, fish2, CalCirclePoint(fightFish, blockUp), 2);
            //Helpers.PoseToPose(ref decisions[1], fish2, CalCirclePoint(fightFish, blockUp), CorrectRad(fightFish.BodyDirectionRad + (float)Math.PI), 80, 150, 100, ref timeForPoseToPose[2]);
            //JudgeFish(ref decisions, fish3, CalCirclePoint(fightFish, blockMiddle), 3);
            //Helpers.PoseToPose(ref decisions[2], fish3, CalCirclePoint(fightFish, blockMiddle), CorrectRad(fightFish.BodyDirectionRad + (float)Math.PI), 80, 150, 100, ref timeForPoseToPose[3]);
            //JudgeFish(ref decisions, fish4, CalCirclePoint(fightFish, blockDown), 4);
            //Helpers.PoseToPose(ref decisions[3], fish4, CalCirclePoint(fightFish, blockDown), CorrectRad(fightFish.BodyDirectionRad + (float)Math.PI), 80, 150, 100, ref timeForPoseToPose[4]);
            //#endregion

            decisions[1].VCode = 3;
            decisions[1].TCode = 2;
            decisions[2].VCode = 3;
            decisions[2].TCode = 1;
            decisions[3].VCode = 3;
            decisions[3].TCode = 0;
            return decisions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;
using URWPGSim2D.StrategyHelper;
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
            return "生存挑战 Test Team";
        }
        public static int[] timeForPoseToPose = new int[5];
        public static int IsDirectionRight(float a, float b)
        {
            if (a > Math.PI) a -= (float)(2 * Math.PI);
            if (b > Math.PI) b -= (float)(2 * Math.PI);
            if (a < -Math.PI) a += (float)(2 * Math.PI);
            if (b < -Math.PI) b += (float)(2 * Math.PI);
            if (a - b > 0.15) return 1;//a在b右边
            else if (a - b < -0.15) return -1; //a在b左边
            else return 0;
        }
        public static bool AllEqual(int[] group, int value, int start, int end)//核对group数组从start到end元素与value相等
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
        public static void StopFish(ref Decision decision, int i)
        {
            decision.VCode = 0;
            decision.TCode = 7;
            timeForPoseToPose[i] = 0;
        }
        public static float GetVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2d) + Math.Pow((a.Z - b.Z), 2d)));
        }
        public static int GetFishArea(xna.Vector3 a)
        {
            if (a.X < 250 && a.X > -250) 
            {
                if (a.Z > 900)
                    return 8;
                else if (a.Z > 200)
                    return 7;
                else if (a.Z > -500)
                    return 6;
                else
                    return 5;
            }
            if (a.X < 0 && a.Z > 0)
                return 3;
            else if (a.X > 0 && a.Z < 0)
                return 1;
            else if (a.X < 0 && a.Z < 0)
                return 4;
            else 
                return 2;
        }
        //public static int CalculateFishToCatch(Mission mission, int teamId)
    

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
            int by2 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish2Caught"]);
            int by3 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish3Caught"]);
            int by4 = Convert.ToInt32(mission.HtMissionVariables["IsYellowFish4Caught"]);
            int br2 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish2Caught"]);
            int br3 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish3Caught"]);
            int br4 = Convert.ToInt32(mission.HtMissionVariables["IsRedFish4Caught"]);
            //RoboFish 
            return decisions;
        }
    }
}

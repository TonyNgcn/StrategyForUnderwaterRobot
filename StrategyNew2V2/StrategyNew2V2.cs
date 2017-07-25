using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
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
        /// 

        private Decision[] decisions = null;

        /// <summary>
        /// 获取队伍名称 在此处设置参赛队伍的名称
        /// </summary>
        /// <returns>队伍名称字符串</returns>
        public string GetTeamName()
        {
            return "Team First";
        }
        public int IsDirectionRight(float a, float b)
        {
            if (a > Math.PI) a -= (float)(2 * Math.PI);
            if (b > Math.PI) b -= (float)(2 * Math.PI);
            if (a < -Math.PI) a += (float)(2 * Math.PI);
            if (b < -Math.PI) b += (float)(2 * Math.PI);
            if (a - b > 0.15) return 1;//a在b右边
            else if (a - b < -0.15) return -1; //a在b左边
            else return 0;
        }
        public static float GetVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2d) + Math.Pow((a.Z - b.Z), 2d)));
        }
        #region 判断球在哪个区域的函数
        public int Field(Vector3 PositionMm)
        {
            int flag = 0;

            if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) //区域1
                flag = 1;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= -1000 && PositionMm.Z <= -500) //区域2
                flag = 2;

            else if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域3
                flag = 3;

            else if (PositionMm.X >= -1150 && PositionMm.X <= -940 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域4
                flag = 4;

            else if ((PositionMm.X >= -940 && PositionMm.X <= 940) || (PositionMm.X >= -1150 && PositionMm.X <= 1150 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) || (PositionMm.X >= -1150 && PositionMm.X <= 1150 && PositionMm.Z >= 550 && PositionMm.Z <= 1000)) //区域5
                flag = 5;

            else if (PositionMm.X >= 940 && PositionMm.X <= 1150 && PositionMm.Z >= -500 && PositionMm.Z <= 500) //区域6
                flag = 6;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= -500 && PositionMm.Z <= 500) //区域7
                flag = 7;

            else if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= 550 && PositionMm.Z <= 1000) //区域8
                flag = 8;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= 500 && PositionMm.Z <= 1000) //区域9
                flag = 9;

            return flag;
        }
        #endregion
        public int myflag = 0;
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
            Ball[] ballGroup = new Ball[9];
            for (int i = 1; i < 9; i++)
                ballGroup[i] = mission.EnvRef.Balls[i];
            RoboFish myFish1 = mission.TeamsRef[teamId].Fishes[0];
            RoboFish myFish2 = mission.TeamsRef[teamId].Fishes[1];
            Vector3 goal;
            int fish_field = Field(myFish2.PositionMm);

            if (mission.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.LEFT)  //在左半场
            {
                switch (fish_field)
                {
                    case 5:
                        goal = new Vector3(1500, 0, -800);
                        Helpers.Dribble(ref decisions[1], myFish2, goal, 0, 20f, 30f, 150f, 15, 10, 15, 100, false);
                        break;
                    case 2:
                        goal = new Vector3(1500, 0, -800);
                        Vector3 goal2 = new Vector3(1500,0,-1000);
                        if (myflag == 0) 
                            Helpers.Dribble(ref decisions[1], myFish2, goal, 0, 8f, 10f, 150f, 14, 10, 15, 100, false);
                        if (IsDirectionRight(myFish2.BodyDirectionRad, 0) == 0 && GetVectorDistance(myFish2.PositionMm, goal) < 200)
                            myflag = 1;
                        if (myflag == 1)
                            Helpers.Dribble(ref decisions[1], myFish2, goal, (float)-Math.PI / 2, 8f, 10f, 150f, 14, 10, 15, 100, false);
                        if (IsDirectionRight(myFish2.BodyDirectionRad, (float)-Math.PI / 2) == 0 && GetVectorDistance(myFish2.PositionMm, goal2) < 250)
                            myflag = 2;
                        if (myflag == 2) 
                        {
                            decisions[1].TCode = 5;
                            decisions[1].VCode = 14;
                        }
                        break;
                }
            }
        return decisions;
          
        }
    }
}

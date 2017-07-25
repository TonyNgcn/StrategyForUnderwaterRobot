using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using URWPGSim2D.Common;
using URWPGSim2D.StrategyLoader;

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
        #region 在区域中找出高分球
        public int Find3Ball(Ball[] ballGroup)//可能死循环
        {
            if (Field(ballGroup[0].PositionMm) == 5)
                return 0;
            if (Field(ballGroup[1].PositionMm) == 5)
                return 1;
            if (Field(ballGroup[2].PositionMm) == 5)
                return 2;
            if (Field(ballGroup[0].PositionMm) == 2)
                return 0;
            if (Field(ballGroup[1].PositionMm) == 2)
                return 1;
            if (Field(ballGroup[2].PositionMm) == 2)
                return 2;
            if (Field(ballGroup[0].PositionMm) == 8)
                return 0;
            if (Field(ballGroup[1].PositionMm) == 8)
                return 1;
            if (Field(ballGroup[2].PositionMm) == 8)
                return 2;
            if (Field(ballGroup[0].PositionMm) == 6)
                return 0;
            if (Field(ballGroup[1].PositionMm) == 6)
                return 1;
            if (Field(ballGroup[2].PositionMm) == 6)
                return 2;
            if (Field(ballGroup[0].PositionMm) == 7)
                return 0;
            if (Field(ballGroup[1].PositionMm) == 7)
                return 1;
            if (Field(ballGroup[2].PositionMm) == 7)
                return 2;
            return 10;
        }
        public int Find2Ball(Ball[] ballGroup)
        #endregion
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
            int b0_l = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]); //判断球是否已经得分的状态量，1为得分，0为没得分
            int b1_l = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
            int b2_l = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
            int b3_l = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
            int b4_l = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
            int b5_l = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
            int b6_l = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
            int b7_l = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
            int b8_l = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);




            return decisions;
        }
    }
}

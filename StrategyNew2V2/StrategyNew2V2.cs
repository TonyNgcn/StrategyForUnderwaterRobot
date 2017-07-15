using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using xna = Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
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

 

        private Decision[] decisions = null;

        public string GetTeamName()
        {
            return "抢球博弈 Test Team";
        }


        #region 自定义数据的声明

        private Vector3 v3 = new Vector3(-1300, 0, 1000);
        private Vector3 v4 = new Vector3(-1300, 0, -1000);

        private Vector3 LeftCourt_BottomCorner = new Vector3(-1500, 0, 1000);  //左场地最下角
        private Vector3 LeftCourt_TopCorner = new Vector3(-1500, 0, -1000); //左场地最上角

        private Vector3 LeftCourt_DownMidpoint = new Vector3(-1500, 0, 50); //最左边场地的中点
        private Vector3 LeftCourt_UpMidpoint = new Vector3(-1500, 0, -50);

        private Vector3 p1 = new Vector3(-1100, 0, 50); //鱼1需要到达的点
        private Vector3 p2 = new Vector3(-1300, 0, -50);  //鱼2需要到达的点
        private Vector3 p = new Vector3(-1300, 0, 0); //鱼1需要到达的点

        private Vector3 LeftGoal_RightTopCorner = new Vector3(-800, 0, -700); //左球门右上角
        private Vector3 LeftGoal_RightBottomCorner = new Vector3(-800, 0, 700); //左球门右下角

        private Vector3 LeftGoal_LeftTopCorner = new Vector3(-1500, 0, -440); //右球门右上角
        private Vector3 LeftGoal_LeftBottomCorner = new Vector3(-1500, 0, 440); //右球门右下角


        private Vector3 ball0;   //球1的坐标
        private Vector3 ball1;   //球2的坐标
        private Vector3 ball2;   //球3的坐标
        private Vector3 ball3;   //球4的坐标
        private Vector3 ball4;   //球5的坐标
        private Vector3 ball5;   //球6的坐标
        private Vector3 ball6;   //球7的坐标
        private Vector3 ball7;   //球8的坐标
        private Vector3 ball8;   //球9的坐标

        private RoboFish My_fish1;
        private RoboFish My_fish2;
        private RoboFish Enemy_fish1;
        private RoboFish Enemy_fish2;


        private int MyScore;

        private Vector3 fish1_head;  //鱼头1的坐标
        private Vector3 fish1_body;  //鱼体1的坐标

        private Vector3 fish2_head;  //鱼头2的坐标
        private Vector3 fish2_body;  //鱼体2的坐标

        private Vector3 fish1_Position; //鱼1的头部刚体中心
        private Vector3 fish2_Position; //鱼2的头部刚体中心

        private float fish1_velocity;  //鱼1当前的速度
        private float fish2_velocity;  //鱼2当前的速度

        private float fish1_BodyDirectionRad;//鱼1的方向
        private float fish2_BodyDirectionRad; //鱼2的方向


        private float R;   //一个范围的半径

        private int CycleTime = 100;  //仿真周期

        private int time = 0;
        private int time1 = 0;  //PoseToPose的times
        private int time2 = 0;
        private int time3 = 0;
        private int time4 = 0;
        private int time5 = 0;
        private int time6 = 0;
        private int time7 = 0;
        private int time8 = 0;

        public double distance(Vector3 temp1, Vector3 temp2)   //点与点的最短距离
        {
            return Math.Sqrt(Math.Pow(temp1.X - temp2.X, 2.0) + Math.Pow(temp1.Z - temp2.Z, 2.0));
        }
        #endregion


        private static int flag = 3;

        public void JinQiu(Mission mission, int teamId) //鱼1鱼2按照一定动作推进球门
        {

            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];

            MyScore = mission.TeamsRef[teamId].Para.Score;

            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_body = new Vector3((mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].Z) / 2);
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_body = new Vector3((mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].Z) / 2);
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;

            fish1_velocity = mission.TeamsRef[teamId].Fishes[0].AngularVelocityRadPs;
            fish2_velocity = mission.TeamsRef[teamId].Fishes[1].AngularVelocityRadPs;

            fish1_BodyDirectionRad = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            fish2_BodyDirectionRad = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;

            R = mission.EnvRef.Balls[0].RadiusMm;  //球的半径

            ball0 = mission.EnvRef.Balls[0].PositionMm;  //下面都是球的中心坐标点
            ball1 = mission.EnvRef.Balls[1].PositionMm;
            ball2 = mission.EnvRef.Balls[2].PositionMm;
            ball3 = mission.EnvRef.Balls[3].PositionMm;
            ball4 = mission.EnvRef.Balls[4].PositionMm;
            ball5 = mission.EnvRef.Balls[5].PositionMm;
            ball6 = mission.EnvRef.Balls[6].PositionMm;
            ball7 = mission.EnvRef.Balls[7].PositionMm;
            ball8 = mission.EnvRef.Balls[8].PositionMm;

            Vector3 point1 = new Vector3(-1500, 0, 532);
            Vector3 point2 = new Vector3(-1500, 0, -532);
            Helpers.PoseToPose(decisions[0],My_fish1,point11,(float)(-Math.PI/2),10f,100f,100,ti)
            #region  左球门进球算法

            //switch (flag)
            //{
            //    //case 0: //调整位置
            //    //    Helpers.Dribble(ref decisions[0], My_fish1, v3, (float)(Math.PI * 3 / 4.0), 10, 5, 200, 12, 8, 15, 100, true);
            //    //    Helpers.Dribble(ref decisions[1], My_fish2, v4, (float)(Math.PI * 3 / -4.0), 10, 5, 200, 12, 8, 15, 100, true);
            //    //    if (distance(fish1_head, v3) < R && distance(fish2_head, v4) < R)
            //    //        flag++;
            //    //    break;

            //    //case 1: //鱼头铲在最边角中
            //    //    Helpers.Dribble(ref decisions[0], My_fish1, LeftCourt_BottomCorner, (float)(Math.PI * 3 / 4.0), 10, 5, 200, 12, 8, 15, 100, true);
            //    //    Helpers.Dribble(ref decisions[1], My_fish2, LeftCourt_TopCorner, (float)(Math.PI * 3 / -4.0), 10, 5, 200, 12, 8, 15, 100, true);
            //    //    if (distance(fish1_head, LeftCourt_BottomCorner) < R && distance(fish2_head, LeftCourt_TopCorner) < R)
            //    //        flag++;
            //    //    break;

            //    //case 2: //每条鱼将两个球贴壁铲到球门附近
            //    //    decisions[0].TCode = 8;
            //    //    decisions[0].VCode = 11;
            //    //    decisions[1].TCode = 6;
            //    //    decisions[1].VCode = 11;
            //    //    if (fish1_head.Z <= 40)
            //    //    {
            //    //        decisions[0].VCode = 0;
            //    //        decisions[0].TCode = 6;
            //    //    }
            //    //    if (fish2_head.Z >= -40)
            //    //    {
            //    //        decisions[1].VCode = 0;
            //    //        decisions[1].TCode = 8;
            //    //    }
            //    //    if (fish1_head.Z <= 40 && fish2_head.Z >= -40)
            //    //        flag++;
            //    //    break;

            //    case 3: //调整位置
            //        Helpers.Dribble(ref decisions[0], My_fish1, LeftCourt_DownMidpoint, (float)(Math.PI), 10, 5, 200, 10, 8, 15, 100, true);
            //        Helpers.Dribble(ref decisions[1], My_fish2, LeftCourt_UpMidpoint, (float)(Math.PI), 10, 5, 200, 10, 8, 15, 100, true);
            //        if ((float)(Math.PI) + fish1_BodyDirectionRad < (float)(Math.PI / 5.0) && ((float)(Math.PI) - fish2_BodyDirectionRad) < (float)(Math.PI / 5.0))
            //            flag++;
            //        break;

            //    case 4: //鱼2位置不变，鱼1将球横推进球门
            //        Helpers.Dribble(ref decisions[0], My_fish1, p, (float)(Math.PI / -4), 10, 5, 200, 2, 1, 15, 100, true);
            //        Helpers.Dribble(ref decisions[1], My_fish2, p, (float)(Math.PI / 4), 10, 5, 200, 2, 1, 15, 100, true);
            //        if (fish1_BodyDirectionRad < 0 && fish1_BodyDirectionRad > (Math.PI / -4))
            //            decisions[0].VCode = 0;
            //        if (fish2_BodyDirectionRad > 0 && fish2_BodyDirectionRad < (Math.PI / 4))
            //            decisions[1].VCode = 0;
            //        if (decisions[1].VCode == 0 && decisions[0].VCode == 0)
            //            flag++;
            //        break;

            //    case 5:
            //        decisions[0].VCode = 1;
            //        decisions[0].TCode = 14;
            //        decisions[1].VCode = 1;
            //        decisions[1].TCode = 0;
            //        if (fish1_BodyDirectionRad < 0 && fish1_BodyDirectionRad > (Math.PI / -4))
            //            decisions[0].VCode = 0;
            //        if (fish2_BodyDirectionRad > 0 && fish2_BodyDirectionRad < (Math.PI / 4))
            //            decisions[1].VCode = 0;
            //        if (fish1_velocity == 0 && fish2_velocity == 0)
            //            flag++;
            //        break;
            //        Helpers.Dribble(ref decisions[0], My_fish1, p, (float)(Math.PI * 3 / -4), 10, 5, 200, 2, 1, 15, 100, true);
            //        Helpers.Dribble(ref decisions[1], My_fish2, p, (float)(Math.PI * 3 / 4), 10, 5, 200, 2, 1, 15, 100, true);
            //        if (fish1_BodyDirectionRad < (float)(Math.PI * -3 / 4) && fish1_BodyDirectionRad > (float)(-Math.PI))
            //            decisions[0].VCode = 0;
            //        if (fish2_BodyDirectionRad > (float)(Math.PI * 3 / 4) && fish2_BodyDirectionRad < (float)(Math.PI))
            //            decisions[1].VCode = 0;
            //        if (decisions[1].VCode == 0 && decisions[0].VCode == 0)
            //            flag++;
            //        break;

            //    case 6: //鱼2位置不变，鱼1将球横推进球门
            //        Helpers.Dribble(ref decisions[0], My_fish1, p1, (float)(Math.PI / -4), 10, 5, 200, 2, 1, 15, 100, true);
            //        Helpers.Dribble(ref decisions[1], My_fish2, p1, (float)(Math.PI / 4), 10, 5, 200, 2, 1, 15, 100, true);
            //        if (fish1_BodyDirectionRad < 0 && fish1_BodyDirectionRad > (Math.PI / -4))
            //            decisions[0].VCode = 0;
            //        if (fish2_BodyDirectionRad > 0 && fish2_BodyDirectionRad < (Math.PI / 4))
            //            decisions[1].VCode = 0;
            //        break;
            //}
            #endregion


        }

        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }

            mission.CommonPara.MsPerCycle = 100;

            if (mission.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.LEFT)  //在左半场
            {
                JinQiu(mission, teamId);  //鱼1鱼2按照一定动作推进球门
            }
           

            return decisions;
        }
    }
}

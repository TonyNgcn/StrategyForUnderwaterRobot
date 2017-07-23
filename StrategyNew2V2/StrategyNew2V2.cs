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



        private Decision[] decisions = null;

        public string GetTeamName()
        {
            return "白鲸华尔兹";
        }


        #region 自定义数据的声明

        private xna.Vector3 v3 = new xna.Vector3(-1300, 0, 1000);
        private xna.Vector3 v4 = new xna.Vector3(-1300, 0, -1000);

        private xna.Vector3 LeftCourt_BottomCorner = new xna.Vector3(-1500, 0, 1000);  //左场地最下角
        private xna.Vector3 LeftCourt_TopCorner = new xna.Vector3(-1500, 0, -1000); //左场地最上角
        private xna.Vector3 RightCourt_BottomCorner = new xna.Vector3(1500, 0, 1000);  //右场地最下角
        private xna.Vector3 RightCourt_TopCorner = new xna.Vector3(1500, 0, -1000); //右场地最上角

        //private xna.Vector3 LeftCourt_DownMidpoint = new xna.Vector3(-1500, 0, 50); //最左边场地的中点
        //private xna.Vector3 LeftCourt_UpMidpoint = new xna.Vector3(-1500, 0, -50);


        private xna.Vector3 LeftCourt_DownMidpoint = new xna.Vector3(-1500, 0, 50); //最左边场地的中点
        private xna.Vector3 LeftCourt_UpMidpoint = new xna.Vector3(-1500, 0, -50);

        private xna.Vector3 p1 = new xna.Vector3(-1100, 0, 50); //鱼1需要到达的点
        private xna.Vector3 p2 = new xna.Vector3(-1300, 0, -50);  //鱼2需要到达的点
        private xna.Vector3 p = new xna.Vector3(-1300, 0, 0); //鱼1需要到达的点

        private xna.Vector3 LeftGoal_RightTopCorner = new xna.Vector3(-800, 0, -700); //左球门右上角
        private xna.Vector3 LeftGoal_RightBottomCorner = new xna.Vector3(-800, 0, 700); //左球门右下角

        private xna.Vector3 LeftGoal_LeftTopCorner = new xna.Vector3(-1500, 0, -440); //右球门右上角
        private xna.Vector3 LeftGoal_LeftBottomCorner = new xna.Vector3(-1500, 0, 440); //右球门右下角


        private xna.Vector3 RightCourt_DownMidpoint = new xna.Vector3(1500, 0, 50); //最右边场地的中点
        private xna.Vector3 RightCourt_UpMidpoint = new xna.Vector3(1500, 0, -50);


        private static xna.Vector3 LeftCourt_Midpoint = new xna.Vector3(-1500, 0, 0);
        private xna.Vector3 temp1 = new xna.Vector3(-1300, 0, 0);
        private xna.Vector3 temp2 = new xna.Vector3(-1150, 0, 0);



        private xna.Vector3 ball0;   //球1的坐标
        private xna.Vector3 ball1;   //球2的坐标
        private xna.Vector3 ball2;   //球3的坐标
        private xna.Vector3 ball3;   //球4的坐标
        private xna.Vector3 ball4;   //球5的坐标
        private xna.Vector3 ball5;   //球6的坐标
        private xna.Vector3 ball6;   //球7的坐标
        private xna.Vector3 ball7;   //球8的坐标
        private xna.Vector3 ball8;   //球9的坐标

        private RoboFish My_fish1;
        private RoboFish My_fish2;
        private RoboFish Enemy_fish1;
        private RoboFish Enemy_fish2;


        private int MyScore;

        private xna.Vector3 fish1_head;  //鱼头1的坐标
        private xna.Vector3 fish1_body;  //鱼体1的坐标

        private xna.Vector3 fish2_head;  //鱼头2的坐标
        private xna.Vector3 fish2_body;  //鱼体2的坐标

        private xna.Vector3 fish1_Position; //鱼1的头部刚体中心
        private xna.Vector3 fish2_Position; //鱼2的头部刚体中心

        private float fish1_velocity;  //鱼1当前的速度
        private float fish2_velocity;  //鱼2当前的速度

        private float fish1_BodyDirectionRad;//鱼1的方向
        private float fish2_BodyDirectionRad; //鱼2的方向


        private float R = 58;   //一个范围的半径

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
        private int time9 = 0;

        //StreamWriter log = new StreamWriter("C:/Users/陈俊杰/Desktop/2D仿真/抢球博弈/Log.txt", false);  //可用于打印关键文件 , true:表示不删除以前的记录；false：表示删除以前记录

        public double distance(xna.Vector3 temp1, xna.Vector3 temp2)   //点与点的最短距离
        {
            return Math.Sqrt(Math.Pow(temp1.X - temp2.X, 2.0) + Math.Pow(temp1.Z - temp2.Z, 2.0));
        }
        #endregion


        private static int flag = 1;

        private static int ball0_field = 0; //球0在哪个区域内
        private static int ball1_field = 0; //球1在哪个区域内
        private static int ball2_field = 0; //球2在哪个区域内
        private static int ball3_field = 0; //球3在哪个区域内
        private static int ball4_field = 0; //球4在哪个区域内
        private static int ball5_field = 0; //球5在哪个区域内
        private static int ball6_field = 0; //球6在哪个区域内
        private static int ball7_field = 0; //球7在哪个区域内
        private static int ball8_field = 0; //球8在哪个区域内

        private static int fish1_field = 0; //判断鱼1在哪个区域


        #region 判断球在哪个区域的函数
        public int Field(xna.Vector3 PositionMm)
        {
            int flag = 0;

            if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) //区域1
                flag = 1;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) //区域2
                flag = 2;

            else if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域3
                flag = 3;

            else if (PositionMm.X >= -1150 && PositionMm.X <= -940 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域4
                flag = 4;

            else if ((PositionMm.X >= -940 && PositionMm.X <= 940) || (PositionMm.X >= -1150 && PositionMm.X <= 1150 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) || (PositionMm.X >= -1150 && PositionMm.X <= 1150 && PositionMm.Z >= 550 && PositionMm.Z <= 1000)) //区域5
                flag = 5;

            else if (PositionMm.X >= 940 && PositionMm.X <= 1150 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域6
                flag = 6;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域7
                flag = 7;

            else if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= 550 && PositionMm.Z <= 1000) //区域8
                flag = 8;

            else if (PositionMm.X >= 1150 && PositionMm.X <= 1500 && PositionMm.Z >= 550 && PositionMm.Z <= 1000) //区域9
                flag = 9;

            return flag;
        }
        #endregion


        #region 鱼1 下半场进攻
        public void Fish1(Mission mission, int teamId, int fish_field)
        {
            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_BodyDirectionRad = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            xna.Vector3 goal;

            ball0_field = Field(ball0); //判断球在哪个区域
            ball1_field = Field(ball1);
            ball2_field = Field(ball2);
            ball3_field = Field(ball3);
            ball4_field = Field(ball4);
            ball5_field = Field(ball5);
            ball6_field = Field(ball6);
            ball7_field = Field(ball7);
            ball8_field = Field(ball8);

            ball0 = mission.EnvRef.Balls[0].PositionMm;  //下面都是球的中心坐标点
            ball1 = mission.EnvRef.Balls[1].PositionMm;
            ball2 = mission.EnvRef.Balls[2].PositionMm;
            ball3 = mission.EnvRef.Balls[3].PositionMm;
            ball4 = mission.EnvRef.Balls[4].PositionMm;
            ball5 = mission.EnvRef.Balls[5].PositionMm;
            ball6 = mission.EnvRef.Balls[6].PositionMm;
            ball7 = mission.EnvRef.Balls[7].PositionMm;
            ball8 = mission.EnvRef.Balls[8].PositionMm;


            xna.Vector3 enemyDownGoal;

            if (mission.TeamsRef[teamId].Para.MyHalfCourt == HalfCourt.LEFT)  //在左半场
            {
                int b0_l = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]); //判断球是否已经得分的状态量，1为得分，0为没得分
                int b1_l = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
                int b2_l = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
                int b3_l = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
                int b4_l = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
                int b5_l = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
                int b6_l = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
                int b7_l = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
                int b8_l = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);

                goal = new xna.Vector3(-1000, 0, -500);

                if (ball3_field == 3 && b3_l == 0)
                {
                    float dir3 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(goal - ball3)); //球3和对应的洞的连线的弧度
                    xna.Vector3 destPtMm3 = new xna.Vector3(ball3.X - 65 * (float)Math.Cos(dir3), 0, ball3.Z - 65 * (float)Math.Sin(dir3));//球3的顶球点

                    switch (fish_field)
                    {

                        case 2:
                        case 5:
                        case 9:
                            Helpers.Dribble(ref decisions[0], My_fish1, LeftCourt_BottomCorner, (float)(Math.PI), 8, 12, 10, 14, 13, 5, 100, true);
                            break;

                        case 1:
                        case 3:
                        case 8:
                            Helpers.Dribble(ref decisions[0], My_fish1, destPtMm3, dir3, 15, 10, 140, 8, 6, 5, 100, false);
                            break;

                        //case 7:
                        //case 6:
                        //    Helpers.Dribble(ref decisions[0], My_fish1, goal, (float)(Math.PI / -2), 8, 12, 10, 8, 6, 5, 100, true);
                        //    if (Math.Abs(fish1_BodyDirectionRad - Math.PI / -2) <= (Math.PI / 36) && distance(fish1_head, goal) < 50)  //如果在方向正确并且距离很近的时候，那就处于静止状态
                        //    {
                        //        decisions[0].VCode = 0;
                        //        decisions[0].TCode = 7;
                        //    }
                        //    break;


                        //case 4:
                        //    Helpers.Dribble(ref decisions[0], My_fish1, LeftCourt_BottomCorner, (float)(Math.PI / 2), 8, 12, 10, 14, 13, 5, 100, true);
                        //    break;

                        default:
                            break;

                    }
                }

            }
            else //在右半场
            {
                int b0_r = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Right_Status"]);
                int b1_r = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Right_Status"]);
                int b2_r = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Right_Status"]);
                int b3_r = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Right_Status"]);
                int b4_r = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Right_Status"]);
                int b5_r = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Right_Status"]);
                int b6_r = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Right_Status"]);
                int b7_r = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Right_Status"]);
                int b8_r = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Right_Status"]);

                enemyDownGoal = new xna.Vector3(-1500, 0, 630);
                switch (fish_field)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 8:
                    case 9:
                        if (Math.Abs(fish1_BodyDirectionRad - Math.PI) <= (Math.PI / 36) && distance(fish1_head, enemyDownGoal) < 50) //如果在方向正确并且距离很近的时候，那就处于静止状态
                        {
                            decisions[0].VCode = 0;
                            decisions[0].TCode = 7;
                        }
                        else
                            Helpers.Dribble(ref decisions[0], My_fish1, enemyDownGoal, (float)(Math.PI), 8, 12, 10, 14, 13, 5, 100, true);
                        break;

                    case 6:
                    case 7:
                        Helpers.Dribble(ref decisions[0], My_fish1, RightCourt_BottomCorner, (float)(Math.PI / 2), 8, 12, 10, 14, 13, 5, 100, true);
                        break;

                    default:
                        break;

                }
            }
        }

        #endregion



        public Decision[] GetDecision(Mission mission, int teamId)
        {
            // 决策类当前对象第一次调用GetDecision时Decision数组引用为null
            if (decisions == null)
            {// 根据决策类当前对象对应的仿真使命参与队伍仿真机器鱼的数量分配决策数组空间
                decisions = new Decision[mission.CommonPara.FishCntPerTeam];
            }

            mission.CommonPara.MsPerCycle = 100;

            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];
            Enemy_fish1 = mission.TeamsRef[(1 + teamId) % 2].Fishes[0];  //敌方鱼1
            Enemy_fish2 = mission.TeamsRef[(1 + teamId) % 2].Fishes[1];  //敌方鱼2

            xna.Vector3 enemyFish1_head = Enemy_fish1.PositionMm;
            xna.Vector3 enemyFish2_head = Enemy_fish2.PositionMm;


            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].Z) / 2);
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].Z) / 2);
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;

            fish1_velocity = mission.TeamsRef[teamId].Fishes[0].AngularVelocityRadPs;
            fish2_velocity = mission.TeamsRef[teamId].Fishes[1].AngularVelocityRadPs;

            fish1_BodyDirectionRad = mission.TeamsRef[teamId].Fishes[0].BodyDirectionRad;
            fish2_BodyDirectionRad = mission.TeamsRef[teamId].Fishes[1].BodyDirectionRad;


            //ball0 = mission.EnvRef.Balls[0].PositionMm;  //下面都是球的中心坐标点
            //ball1 = mission.EnvRef.Balls[1].PositionMm;
            //ball2 = mission.EnvRef.Balls[2].PositionMm;
            //ball3 = mission.EnvRef.Balls[3].PositionMm;
            //ball4 = mission.EnvRef.Balls[4].PositionMm;
            //ball5 = mission.EnvRef.Balls[5].PositionMm;
            //ball6 = mission.EnvRef.Balls[6].PositionMm;
            //ball7 = mission.EnvRef.Balls[7].PositionMm;
            //ball8 = mission.EnvRef.Balls[8].PositionMm;


            //int b0_l = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]); //判断球是否已经得分的状态量，1为得分，0为没得分
            //int b1_l = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
            //int b2_l = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
            //int b3_l = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
            //int b4_l = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
            //int b5_l = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
            //int b6_l = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
            //int b7_l = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
            //int b8_l = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);

            //int b0_r = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Right_Status"]);
            //int b1_r = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Right_Status"]);
            //int b2_r = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Right_Status"]);
            //int b3_r = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Right_Status"]);
            //int b4_r = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Right_Status"]);
            //int b5_r = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Right_Status"]);
            //int b6_r = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Right_Status"]);
            //int b7_r = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Right_Status"]);
            //int b8_r = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Right_Status"]);

            //log.WriteLine("b0_l: " + b0_l+ "\tb1_l: " + b1_l + "\tb2_l: " + b2_l + "\tb3_l: " + b3_l + "\tb4_l: " + b4_l + "\tb5_l: " + b5_l + "\tb6_l: " + b6_l + "\tb7_l: " + b7_l + "\tb8_l: " + b8_l );
            //log.WriteLine("b0_r: " + b0_r+ "\tb1_r: " + b1_r + "\tb2_r: " + b2_r + "\tb3_r: " + b3_r + "\tb4_r: " + b4_r + "\tb5_r: " + b5_r + "\tb6_r: " + b6_r + "\tb7_r: " + b7_r + "\tb8_r: " + b8_r);



            fish1_field = Field(fish1_Position);

            Fish1(mission, teamId, fish1_field);



            return decisions;
        }
    }
}

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
    class Fish2
    {
        #region 自定义数据的声明

        private static xna.Vector3 LeftCourt_BottomCorner = new xna.Vector3(-1500, 0, 1000);  //左场地最下角
        private static xna.Vector3 LeftCourt_TopCorner = new xna.Vector3(-1500, 0, -1000); //左场地最上角

        private static xna.Vector3 RightCourt_BotttomCorner = new xna.Vector3(1500, 0, 1000);
        private static xna.Vector3 RightCourt_TopCorner = new xna.Vector3(1500, 0, -1000);

        private static xna.Vector3 LeftCourt_DownMidpoint = new xna.Vector3(-1500, 0, 50); //最左边场地的中点
        private static xna.Vector3 LeftCourt_UpMidpoint = new xna.Vector3(-1500, 0, -50);

        private static xna.Vector3 RightCourt_DownMidpoint = new xna.Vector3(1500, 0, 50);
        private static xna.Vector3 RightCourt_UpMidpoint = new xna.Vector3(1500, 0, -50);

        private static xna.Vector3 LeftGoal_RightTopCorner = new xna.Vector3(-1000, 0, -500); //左球门右上角
        private static xna.Vector3 LeftGoal_RightBottomCorner = new xna.Vector3(-1000, 0, 500); //左球门右下角


        private static xna.Vector3 LeftGoal_LeftTopCorner = new xna.Vector3(-1148, 0, -512); //左球门左上角
        private static xna.Vector3 LeftGoal_LeftBottomCorner = new xna.Vector3(-1148, 0, 512); //左球门左下角
        private static xna.Vector3 LeftGoal_Midpoint = new xna.Vector3(-976, 0, 0);

        private static xna.Vector3 LeftGoal_UpMidpoint = new xna.Vector3(-800, 0, -350);   //左球门中间
        private static xna.Vector3 LeftGoal_DownMidpoint = new xna.Vector3(-800, 0, 350);

        private static xna.Vector3 RightGoal_UpMidpoint = new xna.Vector3(800, 0, -350);
        private static xna.Vector3 RightGoal_DownMidpoint = new xna.Vector3(800, 0, 350);
        private static xna.Vector3 RightGoal_Midpoint = new xna.Vector3(976, 0, 0);
        private static xna.Vector3 RightGoal_LeftTopCorner = new xna.Vector3(940, 0, -550);
        private static xna.Vector3 RightGoal_LeftBottomCorner = new xna.Vector3(940, 0, 550);
        private static xna.Vector3 RightGoal_RightTopCorner = new xna.Vector3(1150, 0, -550);
        private static xna.Vector3 RightGoal_RightBottomCorner = new xna.Vector3(1150, 0, 550);

        private static xna.Vector3 LeftCourt_Downpoint = new xna.Vector3(-1500, 0, 512);
        private static xna.Vector3 LeftCourt_Uppoint = new xna.Vector3(-1500, 0, -512);

        private static xna.Vector3 LeftCourt_Midpoint = new xna.Vector3(-1500, 0, 0);
        private static xna.Vector3 RightCourt_Midpoint = new xna.Vector3(1500, 0, 0);

        private static int RemainCycle;
        private static int MsPerCycle;
        private static int EnemyScore;
        private static xna.Vector3 ball0;   //球1的坐标
        private static xna.Vector3 ball1;   //球2的坐标
        private static xna.Vector3 ball2;   //球3的坐标
        private static xna.Vector3 ball3;   //球4的坐标
        private static xna.Vector3 ball4;   //球5的坐标
        private static xna.Vector3 ball5;   //球6的坐标
        private static xna.Vector3 ball6;   //球7的坐标
        private static xna.Vector3 ball7;   //球8的坐标
        private static xna.Vector3 ball8;   //球9的坐标

        private static RoboFish My_fish1;
        private static RoboFish My_fish2;

        private static int Ball0_Right;
        private static int Ball1_Right;
        private static int Ball2_Right;
        private static int Ball3_Right;
        private static int Ball4_Right;
        private static int Ball5_Right;
        private static int Ball6_Right;
        private static int Ball7_Right;
        private static int Ball8_Right;

        private static int Ball0_Left;
        private static int Ball1_Left;
        private static int Ball2_Left;
        private static int Ball3_Left;
        private static int Ball4_Left;
        private static int Ball5_Left;
        private static int Ball6_Left;
        private static int Ball7_Left;
        private static int Ball8_Left;
        private static int MyScore;

        private static xna.Vector3 fish1_head;  //鱼头1的坐标
        private static xna.Vector3 fish1_body;  //鱼体1的坐标

        private static xna.Vector3 fish2_head;  //鱼头2的坐标
        private static xna.Vector3 fish2_body;  //鱼体2的坐标

        private static xna.Vector3 efish1_head;  //敌方鱼头1的坐标
        private static xna.Vector3 efish2_head;  //敌方鱼头2的坐标
        private static xna.Vector3 efish1_Position;
        private static xna.Vector3 efish2_Position;
        private static xna.Vector3 fish1_Position; //鱼1的头部刚体中心
        private static xna.Vector3 fish2_Position; //鱼2的头部刚体中心

        private static float fish1_velocity;  //鱼1当前的速度
        private static float fish2_velocity;  //鱼2当前的速度

        private static float fish1_BodyDirectionRad;//鱼1的方向
        private static float fish2_BodyDirectionRad; //鱼2的方向
        private static float efish1_BodyDirectionRad;
        private static float efish2_BodyDirectionRad;

        //每个动作的标志       
        private static int flag = 0;
        private static int eflag = 0;
        private static int free = 0;
        private static int[] flag_1 = new int[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };  //第一步的动作标志
        private static int[] flag_1_2 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2 = new int[14] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_2 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_3 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_4 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_5 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_6 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static int[] flag_2_7 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private int[] flag_2_8_1 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] flag_2_8_1_1 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_1_2 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_1_3 = new int[5] { 0, 0, 0, 0, 0 };

        private int[] flag_2_8_2 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] flag_2_8_2_1 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_2_2 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_2_3 = new int[5] { 0, 0, 0, 0, 0 };

        private int[] flag_2_8_3 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] flag_2_8_3_1 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_3_2 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_3_3 = new int[5] { 0, 0, 0, 0, 0 };

        private int[] flag_2_8_4 = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private int[] flag_2_8_4_1 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_4_2 = new int[5] { 0, 0, 0, 0, 0 };
        private int[] flag_2_8_4_3 = new int[5] { 0, 0, 0, 0, 0 };



        private static float R;   //一个范围的半径

        private static int CycleTime = 100;  //仿真周期


        private static int time1 = 0;  //PoseToPose的times
        private static int time2 = 0;
        private static int time3 = 0;
        private static int time4 = 0;
        private static int time5 = 0;
        private static int time6 = 0;
        private static int time7 = 0;
        private static int time8 = 0;

        public static double distance(xna.Vector3 temp1, xna.Vector3 temp2)   //点与点的最短距离
        {
            return Math.Sqrt(Math.Pow(temp1.X - temp2.X, 2.0) + Math.Pow(temp1.Z - temp2.Z, 2.0));
        }
        #endregion
        public static void Moving_LeftFirsthalf(Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 自定义数据的定义
            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];

            MyScore = mission.TeamsRef[teamId].Para.Score;

            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].Z) / 2);
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].Z) / 2);
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;

            efish1_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[0];
            efish2_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].PolygonVertices[0];

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

            mission.CommonPara.MsPerCycle = CycleTime;

            #endregion

            #region  鱼2顶1号球
            #region  自定义数据
            xna.Vector3 p6 = new xna.Vector3(ball7.X + 3 * R, 0, ball7.Z + 2 * R);//第7个球的临时点
            xna.Vector3 p5 = new xna.Vector3(ball0.X + 2 * R, 0, ball0.Z + 2 * R);  //第0个球的临时顶球点
            xna.Vector3 p7 = new xna.Vector3(-1144, 0, 0);    //左球门外中心点
            xna.Vector3 p12 = new xna.Vector3(ball0.X + 3 * R, 0, ball0.Z + 3 * R);
            double r2 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball0));  //第0个球到达左上角
            xna.Vector3 p8 = new xna.Vector3(ball0.X - R * (float)(Math.Cos(r2)), 0, ball0.Z - R * (float)(Math.Sin(r2)));  //经过计算的顶球临时目标点


            double r3 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball7));   //第7个球到达左上角
            xna.Vector3 p9 = new xna.Vector3(ball7.X - R * (float)(Math.Cos(r3)), 0, ball7.Z - R * (float)(Math.Sin(r3)));

            double r4 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftGoal_DownMidpoint - ball7));    //第7个球到达球门中心
            xna.Vector3 p10 = new xna.Vector3(ball7.X - R * (float)(Math.Cos(r4)), 0, ball7.Z - R * (float)(Math.Sin(r4)));

            double r5 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftGoal_DownMidpoint - ball0));   //第0个球到达球门中心
            xna.Vector3 p11 = new xna.Vector3(ball0.X - R * (float)(Math.Cos(r5)), 0, ball0.Z + R * (float)(Math.Sin(r5)));

            double r6 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftGoal_DownMidpoint - ball3));   //第3个球到达球门中心
            xna.Vector3 p13 = new xna.Vector3(ball3.X - R * (float)(Math.Cos(r6)), 0, ball3.Z - R * (float)(Math.Sin(r6)));

            double r7 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball1));   //第1个球到达左上角
            xna.Vector3 p14 = new xna.Vector3(ball1.X - R * (float)(Math.Cos(r7)), 0, ball1.Z - R * (float)(Math.Sin(r7)));
            xna.Vector3 a1 = new xna.Vector3(ball1.X + 2 * R, 0, ball1.Z + 2 * R);  //顶第1个球的临时点

            double r8 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball2));   //第2个球到达左上角
            xna.Vector3 p15 = new xna.Vector3(ball2.X - R * (float)(Math.Cos(r8)), 0, ball2.Z - R * (float)(Math.Sin(r8)));
            xna.Vector3 a2 = new xna.Vector3(ball2.X + 2 * R, 0, ball2.Z + 2 * R);  //顶第2个球的临时点

            xna.Vector3 a = new xna.Vector3(-1060, 0, 0);
            xna.Vector3 b = new xna.Vector3(-976, 0, 0);
            xna.Vector3 c = new xna.Vector3(-1272, 0, -660);
            #endregion


            if (flag_2[0] == 0)  //30秒,到达第1个球的临时顶球点附近
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a1, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time1);
                if (distance(fish2_Position, a1) < 100)
                    flag_2[2] = 1;
            }

            if (flag_2[2] == 1)   //判断哪个球没有被顶，顶剩下的球
            {
                if (distance(ball0,efish1_head)>2*R && distance(ball0,efish2_head)>2*R)  //顶第0个球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p5, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time3);
                    if (distance(fish2_Position, p5) < 100)
                        flag_2_2[0] = 1;
                }
                else if (distance(ball2,efish2_head)>2*R && distance(ball2,efish1_head)>2*R)  //顶第2个球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, a2, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, a2) < 100)
                        flag_2_2[1] = 1;
                }
                else
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, p14, (float)(r7 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);//顶1号球
                    if (distance(ball1, efish1_head) < 2 * R || distance(ball1, efish2_head) < 2 * R)  //如果抢1号球的过程中别的鱼在抢球
                        flag_2_2[2] = 1;
                    if (ball1.X < -1150)
                        flag_2_3[0] = 1;
                }
            }
            if (flag_2_2[0] == 1)  //顶0号球
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p8, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);  //顶第0个球
                //Helpers.Dribble(ref decisions[1], My_fish2, ball0, (float)(r2 * Math.PI / 180),10, 11, 200, 14, 12, 15, 100, true);  //顶第0个球
                if (distance(ball0, efish2_head) < 2 * R || distance(ball0, efish1_head) < 2 * R)  //如果抢0号球的过程中别的鱼在抢球
                    flag_2_2[2] = 1;
                if (ball0.X < -1150)
                    flag_2_3[0] = 1;
            }
            if (flag_2_2[1] == 1)  //顶2号球
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p15, (float)(r8 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball2, efish1_head) < 2 * R || distance(ball2, efish2_head) < 2 * R)  //如果抢2号球的过程中别的鱼在抢球
                    flag_2_2[2] = 1;
                if (ball2.X < -1150)
                    flag_2_3[0] = 1;
            }
            if (flag_2_2[2] == 1)   //判断哪个球没有被抢
            {
                if (ball0.X < 940 && distance(efish2_head, ball0) > 2 * R && distance(efish1_head, ball0) > 2 * R && distance(fish1_head, ball0) > 3 * R)  //如果0号球没有被抢,到达0号球附近
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p5, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, p5) < 100)
                        flag_2_2[0] = 1;
                }
                else if(ball1.X<940 && distance(efish1_head,ball1)>2*R && distance(efish2_head,ball1)>2*R && distance(fish1_head,ball1)>3*R)
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, a1, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, a1) < 100)
                        flag_2[3] = 1;
                }
                else if (ball2.X < 940 && distance(efish1_head, ball2) > 2 * R && distance(efish2_head, ball2) > 2 * R && distance(fish1_head, ball2) > 3 * R)  //如果2号球没有被抢，到达2号球附近
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, a2, (float)(3 * Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, a2) < 100)
                        flag_2_2[1] = 1;
                }
            }
            if(flag_2[3]==1)
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p14, (float)(r7 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);//顶1号球
                if (ball1.X < -1150)
                    flag_2_3[0] = 1;
            }
            #region 抢中间的三分球
            if (flag_2_3[0] == 1)   //判断抢中间的哪一个球
            {
                if ((ball0.X < 1000 && ball0.X > -1100) && (distance(efish1_head, ball0) > 2 * R && distance(efish2_head, ball0) > 2 * R) && distance(fish1_head, ball0) > 2 * R)
                {
                    flag_2_3[1] = 1;  //抢第0个球
                }
                else if ((ball1.X < 1000 && ball1.X > -1100) && (distance(efish2_head, ball1) > 2 * R && distance(efish1_head, ball1) > 2 * R) && distance(fish1_head, ball1) > 2 * R)
                {
                    flag_2_3[2] = 1;   //抢第1个球
                }
                else if ((ball2.X < 1000 && ball2.X > -1100) && (distance(efish1_head, ball2) > 2 * R && distance(efish2_head, ball2) > 2 * R) && distance(fish1_head, ball2) > 2 * R)
                {
                    flag_2_3[3] = 1;   //抢第2个球
                }
                else if (ball7.X<1000 && ball7.X>-1100 && distance(efish1_head,ball7)>2*R &&distance(efish2_head,ball7)>2*R)
                    flag_2[4] = 1;
                else
                    flag_2[6] = 1;    //直接顶入角落

            }
            if (flag_2_3[1] == 1)  //如果中间第0个球没有被抢，则去抢第0个球,先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, p5, (float)(r2 * Math.PI / 180), 10.0f, 100, 100, ref time3);
                if (distance(fish2_Position, p5) < 100)
                    flag_2_3[4] = 1;
            }
            if (flag_2_3[2] == 1)  //如果中间第1个球没有被抢，则去抢第1个球，先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a1, (float)(r7 * Math.PI / 180), 10.0f, 100, 100, ref time4);
                if (distance(fish2_Position, a1) < 100)
                    flag_2_3[5] = 1;
            }
            if (flag_2_3[3] == 1)   //如果中间第2个球没有被抢，则去抢第2个球，先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a2, (float)(Math.PI * r8 / 180), 10.0f, 100, 100, ref time5);
                if (distance(fish2_Position, a2) < 100)
                    flag_2[6] = 1;
            }
            if (flag_2_3[4] == 1)   //第0个球到达左上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p8, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball0, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            if (flag_2_3[5] == 1)   //第1个球到达左上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p14, (float)(Math.PI * r7 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball1, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            if (flag_2_3[6] == 1)   //第2个球到达左上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p15, (float)(Math.PI * r8 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball2, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            #endregion
            if (flag_2[4] == 1)  //到达第7个球的临时点p6
            {
                //Helpers.PoseToPose(ref decisions[1], My_fish2, p6, (float)(r3 * Math.PI / 180), 10.0f, 100, 100, ref time4);
                Helpers.PoseToPose(ref decisions[1], My_fish2, p6, -(float)(Math.PI * 5f / 6f), 10.0f, 100, 100, ref time4);
                if (distance(fish2_Position, p6) < 100)
                    flag_2[5] = 1;
            }
            if (flag_2[5] == 1)     //第7个球到达左上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p9, (float)(r3 * Math.PI / 180), 2, 3, 200, 14, 13, 15, 100, true);
                //if (ball7.X > LeftCourt_TopCorner.X && ball7.X < 1300)
                if(distance(ball7,LeftCourt_TopCorner)<2*R)
                    flag_2[6] = 1;
            }
            if (flag_2[6] == 1)  //鱼头顶入角落
            {
                Helpers.Dribble(ref decisions[1], My_fish2, LeftCourt_TopCorner, (float)(Math.PI * 8 / -9), 5, 10, 200, 10, 8, 15, 100, true);
                if (distance(fish2_head, LeftCourt_TopCorner) < 20)
                    flag_2[7] = 1;
            }
            if (flag_2[7] == 1)    //平移，将球顶到球门附近
            {
                decisions[1].VCode = 11;
                decisions[1].TCode = 6;
                if (fish2_head.Z > -400 && fish2_head.Z < -200)
                {
                    decisions[1].VCode = 8;
                    decisions[1].TCode = 6;
                    //Helpers.Dribble(ref decisions[1], My_fish2, LeftCourt_UpMidpoint, (float)Math.PI, 5, 10, 200, 10, 8, 15, 100, true);
                }
                //if ( ((float)(Math.PI) - fish2_BodyDirectionRad) < (float)(Math.PI / 5.0) )
                if (fish2_head.Z > -200 && fish2_head.Z < -100)
                    flag_2[8] = 1;
            }
            if (flag_2[8] == 1)
            {
                decisions[1].VCode = 0;
                decisions[1].TCode = 7;
                if (distance(fish2_head, fish1_head) < 300)
                    flag_2[9] = 1;
            }
            /*if (flag_2[9] == 1)
            {
                Helpers.Dribble(ref decisions[1], My_fish2, fish1_Position, (float)(Math.PI / 2), 2, 3, 10, 12, 10, 15, 100, false);
                if (distance(fish2_Position, b) < 10 && distance(fish1_Position, b) < 10)
                    flag_1[10] = 1;
            }*/

            #endregion
        }

        public static void Moving_RightFirsthalf(Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 自定义数据的定义

            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];

            MyScore = mission.TeamsRef[teamId].Para.Score;

            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].Z) / 2);
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].Z) / 2);
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;

            efish1_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[0];
            efish2_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].PolygonVertices[0];

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

            mission.CommonPara.MsPerCycle = CycleTime;

            #endregion

            #region 自定义数据
            double r0 = xna.MathHelper.ToRadians(Helpers.GetAngleDegree(RightCourt_TopCorner - ball0));  //球0到达左上角
            xna.Vector3 p0 = new xna.Vector3(ball0.X - R * (float)Math.Cos(r0), 0, ball0.Z - R * (float)Math.Sin(r0));
            xna.Vector3 a0 = new xna.Vector3(ball0.X - 2 * R, 0, ball0.Z + 2 * R);

            double r1 = xna.MathHelper.ToRadians(Helpers.GetAngleDegree(RightCourt_TopCorner - ball1));  //球1到达左上角
            xna.Vector3 p1 = new xna.Vector3(ball1.X - R * (float)Math.Cos(r1), 0, ball1.Z - R * (float)Math.Sin(r1));
            xna.Vector3 a1 = new xna.Vector3(ball1.X - 2 * R, 0, ball1.Z + 2 * R);

            double r2 = xna.MathHelper.ToRadians(Helpers.GetAngleDegree(RightCourt_TopCorner - ball2));  //球2到达左上角
            xna.Vector3 p2 = new xna.Vector3(ball2.X - R * (float)Math.Cos(r2), 0, ball2.X - R * (float)Math.Sin(r2));
            xna.Vector3 a2 = new xna.Vector3(ball2.X - 2 * R, 0, ball2.Z + 2 * R);

            double r7 = xna.MathHelper.ToRadians(Helpers.GetAngleDegree(RightCourt_TopCorner - ball7));//球7到达左上角
            xna.Vector3 p7 = new xna.Vector3(ball7.X - R * (float)Math.Cos(r7), 0, ball7.Z - R * (float)Math.Sin(r7));
            xna.Vector3 a7 = new xna.Vector3(ball7.X - 2 * R, 0, ball7.Z - 2 * R);
            xna.Vector3 a = new xna.Vector3(-1060, 0, 0);
            xna.Vector3 b = new xna.Vector3(-976, 0, 0);

            #endregion
            if (flag_2[0] == 0)    //30秒,到达第1个球的临时顶球点附近
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a1, (float)(Math.PI / -4), 10.0f, 100, 100, ref time1);
                if (distance(fish2_Position, a1) < 100)
                    flag_2[1] = 1;
            }
            if (flag_2[1] == 1)    //判断哪个球没有被顶，顶剩下的球
            {
                if ((distance(efish1_head, ball1) < 2 * R && distance(efish2_head, ball2) < 2 * R) || (distance(efish2_head, ball1) < 2 * R && distance(efish1_head, ball2) < 2 * R))  //顶第0个球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, a0, (float)(Math.PI / -4), 10.0f, 100, 100, ref time3);
                    if (distance(fish2_Position, a0) < 100)
                        flag_2_2[0] = 1;
                }
                else if (distance(efish1_head, ball0) < 2 * R && distance(efish2_head, ball1) < 2 * R || (distance(efish2_head, ball0) < 2 * R && distance(efish1_head, ball1) < 2 * R))  //顶第2个球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, a2, (float)(Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, a2) < 100)
                        flag_2_2[1] = 1;
                }
                else
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, p1, (float)(r1 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);//顶1号球
                    if (ball1.X > 1200)
                        flag_2[3] = 1;
                }
            }
            if (flag_2_2[0] == 1)  //顶0号球
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p0, (float)(r0 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);  //顶第0个球
                if (ball0.X > 1200)
                    flag_2[3] = 1;
            }
            if (flag_2_2[1] == 1)  //顶2号球
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p2, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (ball2.X > 1200)
                    flag_2[3] = 1;
            }
            if (flag_2[3] == 1)   //判断第7个球是否有被敌方鱼抢,如果球在右球门偏左的位置,则去抢球,先到达临时点p6;判断中间的球是否还有剩余，有则去抢球
            {
                if (ball7.X < -900 || (distance(efish2_head, ball7) < 4 * R || distance(efish1_head, ball7) < 4 * R))
                    flag_2_3[0] = 1;
                else
                    flag_2[4] = 1;
            }
            if (flag_2_3[0] == 1)   //判断抢中间的哪一个球
            {
                if ((ball0.X < 1000 && ball0.X > -1100) && (distance(efish1_head, ball0) > 2 * R && distance(efish2_head, ball0) > 2 * R) && distance(fish1_head, ball0) > 2 * R)
                {
                    flag_2_3[1] = 1;  //抢第0个球
                }
                else if ((ball1.X < 1000 && ball1.X > -1100) && (distance(efish2_head, ball1) > 2 * R && distance(efish1_head, ball1) > 2 * R) && distance(fish1_head, ball1) < 2 * R)
                {
                    flag_2_3[2] = 1;   //抢第1个球
                }
                else if ((ball2.X < 1000 && ball2.X > -1100) && (distance(efish1_head, ball2) > 2 * R && distance(efish2_head, ball2) > 2 * R) && distance(fish1_head, ball2) < 2 * R)
                {
                    flag_2_3[3] = 1;   //抢第2个球
                }
                else
                    flag_2[6] = 1;    //直接顶第7和第3个球

            }
            if (flag_2_3[1] == 1)  //如果中间第0个球没有被抢，则去抢第0个球,先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a0, (float)(r2 * Math.PI / 180), 10.0f, 100, 100, ref time3);
                if (distance(fish2_Position, a0) < 100)
                    flag_2_3[4] = 1;
            }
            if (flag_2_3[2] == 1)  //如果中间第1个球没有被抢，则去抢第1个球，先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a1, (float)(r1 * Math.PI / 180), 10.0f, 100, 100, ref time4);
                if (distance(fish2_Position, a1) < 100)
                    flag_2_3[5] = 1;
            }
            if (flag_2_3[3] == 1)   //如果中间第2个球没有被抢，则去抢第2个球，先到达临时点
            {
                Helpers.PoseToPose(ref decisions[1], My_fish2, a2, (float)(Math.PI * r2 / 180), 10.0f, 100, 100, ref time5);
                if (distance(fish2_Position, a2) < 100)
                    flag_2[6] = 1;
            }
            if (flag_2_3[4] == 1)   //第0个球到达右上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p0, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball0, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            if (flag_2_3[5] == 1)   //第1个球到达右上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p1, (float)(Math.PI * r1 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball1, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            if (flag_2_3[6] == 1)   //第2个球到达右上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p2, (float)(Math.PI * r2 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                if (distance(ball2, LeftCourt_TopCorner) < 2 * R)
                    flag_2[6] = 1;
            }
            if (flag_2[4] == 1)  //到达临时点p7
            {
                //  Helpers.PoseToPose(ref decisions[1], My_fish2, p6, (float)(r3 * Math.PI / 180), 10.0f, 100, 100, ref time4);
                Helpers.PoseToPose(ref decisions[1], My_fish2, a7, -(float)(Math.PI / 6f), 10.0f, 100, 100, ref time4);
                if (distance(fish2_Position, a7) < 100)
                    flag_2[5] = 1;
            }
            if (flag_2[5] == 1)     //第7个球到达右上角
            {
                Helpers.Dribble(ref decisions[1], My_fish2, p7, (float)(r7 * Math.PI / 180), 2, 3, 200, 14, 13, 15, 100, true);
                if (distance(efish1_head, ball7) < 3 * R)
                {
                    if (fish1_head.X < -1000)
                    {
                        Helpers.PoseToPose(ref decisions[0], My_fish1, LeftCourt_TopCorner, (float)Math.PI / -2, 10.0f, 100, 100, ref time4);

                    }
                    if (distance(ball7, RightCourt_TopCorner) < 2 * R)
                        flag_2[6] = 1;
                }
                if (flag_2[6] == 1)  //鱼头顶入角落
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_TopCorner, (float)(Math.PI / -4), 2, 3, 200, 14, 13, 15, 100, true);
                    if (distance(fish2_head, RightCourt_TopCorner) < 20)
                        flag_2[7] = 1;
                }
                if (flag_2[7] == 1)    //平移，将球顶到球门附近
                {
                    decisions[1].VCode = 11;
                    decisions[1].TCode = 6;
                    if (fish2_head.Z > -300 && fish2_head.Z < -100)
                    {
                        decisions[1].VCode = 8;
                        decisions[1].TCode = 6;
                    }
                    if (fish2_head.Z > -100 && fish2_head.Z < 0)
                        flag_2[8] = 1;
                }
                if (flag_2[8] == 1)
                {
                    decisions[1].VCode = 0;
                    decisions[1].TCode = 7;
                    if (distance(fish2_head, fish1_head) < 250)
                        flag_2[9] = 1;
                }
                if (flag_2[9] == 1)
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, fish1_Position, (float)(Math.PI / 2), 2, 3, 10, 10, 8, 15, 100, false);
                    if (distance(fish2_Position, b) < 10 && distance(fish1_Position, b) < 10)
                        flag_1[10] = 1;
                }
                if (flag_2[10] == 1)
                {

                }
            }
        }


        #region 判断鱼在哪个区域的函数
        public static int Field(xna.Vector3 PositionMm)
        {
            int flag = 0;

            if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) //区域1
                flag = 1;

            else if (PositionMm.X >= 940 && PositionMm.X <= 1500 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) //区域2
                flag = 2;

            else if (PositionMm.X >= -1500 && PositionMm.X <= -1150 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域3
                flag = 3;

            else if (PositionMm.X >= -1150 && PositionMm.X <= -940 && PositionMm.Z >= -550 && PositionMm.Z <= 550) //区域4
                flag = 4;

            else if ((PositionMm.X >= -940 && PositionMm.X <= 940) || (PositionMm.X >= -1150 && PositionMm.X <= -940 && PositionMm.Z >= -1000 && PositionMm.Z <= -550) || (PositionMm.X >= -1150 && PositionMm.X <= -940 && PositionMm.Z >= 550 && PositionMm.Z <= 1000)) //区域5
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


        public static void Moving_LeftSecondhalf(Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 自定义数据的定义

            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];

            MyScore = mission.TeamsRef[teamId].Para.Score;
            EnemyScore = mission.TeamsRef[(1 + teamId) % 2].Para.Score;
            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;

            efish1_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[0];
            efish1_Position = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PositionMm;
            efish2_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[1];
            efish2_Position = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].PositionMm;
            efish1_BodyDirectionRad = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].BodyDirectionRad;
            efish2_BodyDirectionRad = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].BodyDirectionRad;
            
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

            Ball0_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Right_Status"]);
            Ball1_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Right_Status"]);
            Ball2_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Right_Status"]);
            Ball3_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Right_Status"]);
            Ball4_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Right_Status"]);
            Ball5_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Right_Status"]);
            Ball6_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Right_Status"]);
            Ball7_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Right_Status"]);
            Ball8_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Right_Status"]);

            Ball0_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]);
            Ball1_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
            Ball2_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
            Ball3_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
            Ball4_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
            Ball5_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
            Ball6_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
            Ball7_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
            Ball8_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);

            mission.CommonPara.MsPerCycle = CycleTime;

            RemainCycle = mission.CommonPara.RemainingCycles;
            MsPerCycle = mission.CommonPara.MsPerCycle;

            #endregion

            #region 临时点
            xna.Vector3 temp1 = new xna.Vector3(1500, 0, -550); //区域2的临时点
            xna.Vector3 temp2 = new xna.Vector3(1500, 0, 550);  //区域9的临时点

            double r0 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball0));  //0号球
            double w0 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_BottomCorner - ball0));
            xna.Vector3 a0 = new xna.Vector3(ball0.X - R * (float)Math.Cos(r0), 0, ball0.Z - R * (float)Math.Sin(r0));
            xna.Vector3 b0 = new xna.Vector3(ball0.X - R * (float)Math.Cos(w0), 0, ball0.Z - R * (float)Math.Sin(w0));
            xna.Vector3 p0 = new xna.Vector3(ball0.X + 3 * R, 0, ball0.Z + 3 * R);
            xna.Vector3 q0 = new xna.Vector3(ball0.X + 3 * R, 0, ball0.Z - 3 * R);

            double r1 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball1));   //1号球
            double w1 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_BottomCorner - ball1));
            xna.Vector3 a1 = new xna.Vector3(ball1.X - R * (float)Math.Cos(r1), 0, ball1.Z - R * (float)Math.Sin(r1));
            xna.Vector3 b1 = new xna.Vector3(ball1.X - R * (float)Math.Cos(w1), 0, ball1.Z - R * (float)Math.Sin(w1));
            xna.Vector3 p1 = new xna.Vector3(ball1.X + 3 * R, 0, ball1.Z + 3 * R);
            xna.Vector3 q1 = new xna.Vector3(ball1.X + 3 * R, 0, ball1.Z - 3 * R);

            double r2= xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball2));    //2号球
            double w2 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_BottomCorner - ball2));
            xna.Vector3 a2 = new xna.Vector3(ball2.X - R * (float)Math.Cos(r2), 0, ball2.Z - R * (float)Math.Sin(r2));
            xna.Vector3 b2 = new xna.Vector3(ball2.X - R * (float)Math.Cos(w2), 0, ball2.Z - R * (float)Math.Sin(w2));
            xna.Vector3 p2 = new xna.Vector3(ball2.X + 3 * R, 0, ball2.Z + 3 * R);
            xna.Vector3 q2 = new xna.Vector3(ball2.X + 3 * R, 0, ball2.Z - 3 * R);

            double r3 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(LeftCourt_TopCorner - ball3));
            #endregion

            if (flag_1[0] == 0)   //如果三分球在5区域
            {
                if (Field(ball0) == 5) 
                    {
                        if (flag_2_3[0] == 0)
                        {
                            if (distance(ball0, LeftCourt_TopCorner) < distance(ball0, LeftCourt_BottomCorner))
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, p0, (float)Math.PI * 3 / -4, 10.0f, 100, 100, ref time1);
                                if (distance(p0, fish2_Position) < 100)
                                    flag_2_3[1] = 1;
                            }
                            else
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, q0, (float)Math.PI * 3 / 4, 10.0f, 100, 100, ref time1);
                                if (distance(q0, fish2_Position) < 100)
                                    flag_2_3[1] = 1;
                            }
                        }
                    if (flag_2_3[1] == 1) 
                        {
                            if (distance(ball0, LeftCourt_TopCorner) < distance(ball0, LeftCourt_BottomCorner))
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, a0, (float)(r0 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball0.X<-1150)
                                    flag_2_2[1] = 1;
                            }
                            else
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, b0, (float)(w0 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball0.X<-1150)
                                    flag_2_2[1] = 1;
                            }
                        }
                    }

                if (Field(ball1) == 5) 
                    {
                        if (flag_2_3[2] == 0)
                        {
                            if (distance(ball1, LeftCourt_TopCorner) < distance(ball1, LeftCourt_BottomCorner))
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, p1, (float)Math.PI * 3 / -4, 10.0f, 100, 100, ref time2);
                                if (distance(p1, fish2_Position) < 100)
                                    flag_2_3[3] = 1;
                            }
                            else
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, q1, (float)Math.PI * 3 / 4, 10.0f, 100, 100, ref time2);
                                if (distance(q1, fish2_Position) < 100)
                                    flag_2_3[3] = 1;
                            }
                        }
                        if(flag_2_3[3]==1)
                        {
                            if (distance(ball2, LeftCourt_TopCorner) < distance(ball2, LeftCourt_BottomCorner))
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, a1, (float)(r1 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball1.X<-1150)
                                    flag_2_2[2] = 1;
                            }
                            else
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, b1, (float)(w1 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball1.X<-1150)
                                    flag_2_2[2] = 1;
                            }
                        }
                    }
                   
                if (Field(ball2) == 5)
                {
                    if (flag_2_3[4] == 0)
                        {
                            if (distance(LeftCourt_TopCorner, ball2) < distance(LeftCourt_BottomCorner, ball2))
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, p2, (float)Math.PI * 3 / -4, 10.0f, 100, 100, ref time3);
                                if (distance(p2, fish2_Position) < 100)
                                    flag_2_3[5] = 1;
                            }
                            else
                            {
                                Helpers.PoseToPose(ref decisions[1], My_fish2, q2, (float)Math.PI * 3 / 4, 10.0f, 100, 100, ref time3);
                                if (distance(q2, fish2_Position) < 100)
                                    flag_2_3[5] = 1;
                            }
                        }
                    if (flag_2_3[5] == 1)
                        {
                            if (distance(LeftCourt_TopCorner, ball2) < distance(LeftCourt_BottomCorner, ball2))
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, a2, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball2.X < -1150)
                                    flag_1[1] = 1;
                            }
                            else
                            {
                                Helpers.Dribble(ref decisions[1], My_fish2, b2, (float)(w2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                if (ball2.X < -1150)
                                    flag_1[1] = 1;
                            }
                        }
                }
                else
                    flag_1[1] = 1;
            }
            if(flag_1[1]==1)   //如果三分球在2或9区域
            {
                if ((Field(ball0) == 2 && Field(ball1) == 2) || (Field(ball1) == 2 && Field(ball2) == 2) || (Field(ball0) == 2 && Field(ball2) == 2)) //两个或者以上的三分球在2区域
                {
                    if (flag_2[0] == 0)   //顶0号球
                    {
                        if (Field(ball0) == 2 || Field(ball0) == 5)
                        {
                            switch (Field(ball0))
                            {
                                case 5:
                                    if (flag_2_2[0] == 0)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, p0, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time1);   //往左上角顶
                                        if (distance(fish2_Position, p0) < 100)
                                             flag_2_2[1] = 1;
                                    }
                                    if (flag_2_2[1] == 1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, a0, (float)(Math.PI * r0 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball0, LeftCourt_TopCorner) < 2 * R)
                                            flag_2[1] = 1;
                                    }
                                    break;
                                case 2:
                                    if (flag_2_2[2] == 0)   //往右球门的左上角
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftTopCorner, (float)(Math.PI / -3), 5, 10, 200, 14, 13, 15, 100, true);
                                        if (distance(fish2_head, RightGoal_LeftTopCorner) < 100)
                                            flag_2_2[3] = 1;
                                    }
                                    if (flag_2_2[3] == 1)   //往临时点
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, temp1, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                        if (distance(temp1, fish2_head) < 100)
                                            flag_2_2[4] = 1;
                                    }
                                    if (flag_2_2[4] == 1)    //往右上角
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_TopCorner, (float)(Math.PI / -2), 5, 10, 200, 14, 13, 15, 100, true);
                                        if (distance(fish2_head, RightCourt_TopCorner) < 20)
                                            flag_2_2[5] = 1;
                                    }
                                    if (flag_2_2[5] == 1)   //平移将球顶到区域5
                                    {
                                        decisions[1].VCode = 11;
                                        decisions[1].TCode = 6;
                                    }
                                    break;
                                default:break;
                            }
                        }
                        else
                            flag_2[1] = 1;
                    }
                    if (flag_2[1] == 1)   //顶1号球
                    {
                        if (Field(ball1) == 2 || Field(ball1) == 5)
                        {
                            switch (Field(ball1))
                            {
                                case 2:
                                    switch (flag_1[0])
                                    {
                                        case 0:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftTopCorner, (float)(Math.PI / -3), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightGoal_LeftTopCorner) < 100)
                                                flag_1[0]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, temp1, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(temp1, fish2_head) < 100)
                                                flag_1[0]++;
                                            break;
                                        case 2:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_TopCorner, (float)(Math.PI / -2), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightCourt_TopCorner) < 20)
                                                flag_1[0]++;
                                            break;
                                        case 3:
                                            decisions[1].VCode = 11;
                                            decisions[1].TCode = 6;
                                            break;
                                        default: break;
                                    }
                                    break;
                                case 5:
                                    switch (flag_1[1])
                                    {
                                        case 0:
                                            Helpers.PoseToPose(ref decisions[1], My_fish2, p1, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time1);   //往左上角顶
                                            if (distance(fish2_Position, p1) < 100)
                                                flag_1[1]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, a1, (float)(Math.PI * r1 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                            if (distance(ball1, LeftCourt_TopCorner) < 2 * R)
                                                flag_1[1] ++;
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                        }
                        else
                            flag_2[2] = 1;
                    }
                    if (flag_2[2] == 1)    //顶2号球
                    {
                        if (Field(ball2) == 2 || Field(ball2) == 5) 
                        {
                            switch (Field(ball2))
                            {
                                case 2:
                                    switch (flag_1[2])
                                    {
                                        case 0:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftTopCorner, (float)(Math.PI / -3), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightGoal_LeftTopCorner) < 100)
                                                flag_1[2]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, temp1, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(temp1, fish2_head) < 100)
                                                flag_1[2]++;
                                            break;
                                        case 2:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_TopCorner, (float)(Math.PI / -2), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightCourt_TopCorner) < 20)
                                                flag_1[2]++;
                                            break;
                                        case 3:
                                            decisions[1].VCode = 11;
                                            decisions[1].TCode = 6;
                                            break;
                                        default: break;
                                    }
                                    break;
                                case 5:
                                    switch (flag_1[3])
                                    {
                                        case 0:
                                            Helpers.PoseToPose(ref decisions[1], My_fish2, p2, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time3);   //往左上角顶
                                            if (distance(fish2_Position, p2) < 100)
                                                flag_1[3]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, a2, (float)(Math.PI * r2 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                            if (distance(ball2, LeftCourt_TopCorner) < 2 * R)
                                                    flag_1[3]++;
                                            break;
                                        default: break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else if ((Field(ball0) == 9 && Field(ball1) == 9) || (Field(ball0) == 9 && Field(ball2) == 9) || (Field(ball1) == 9 && Field(ball2) == 9))  //两个或者以上的三分球在9区域
                {
                    if (flag_2[0] == 0)   //顶0号球
                    {
                        if (Field(ball0) == 5 || Field(ball0) == 9)
                        {
                            switch (Field(ball0))
                            {
                                case 5:
                                    if (flag_2_2[0] == 0)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, q0, (float)(Math.PI * 3 / 4), 10.0f, 100, 100, ref time1);    //往左下角顶
                                        if (distance(fish2_Position, q0) < 100)
                                                flag_2_2[1] = 1;
                                    }
                                    if (flag_2_2[1] == 1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, b0, (float)(Math.PI * w0 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball0, LeftCourt_BottomCorner) < 2 * R)
                                                flag_2[1] = 1;
                                    }
                                    break;
                                case 9:
                                    if (flag_2_2[5] == 0) //往右球门的左下角
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftBottomCorner, (float)(Math.PI / 3), 5, 10, 200, 14, 13, 15, 100, true);
                                        if (distance(fish2_head, RightGoal_LeftBottomCorner) < 100)
                                            flag_2_2[6] = 1;
                                    }
                                    if (flag_2_2[6] == 1)   //往临时点
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, temp2, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                        if (distance(temp2, fish2_head) < 100)
                                            flag_2_2[7] = 1;
                                    }
                                    if (flag_2_2[7] == 1)   //往右下角
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_BotttomCorner, (float)(Math.PI / 2), 5, 10, 200, 10, 8, 15, 100, true);
                                        if (distance(fish2_head, RightCourt_BotttomCorner) < 20)
                                            flag_2_2[8] = 1;
                                    }
                                    if (flag_2_2[8] == 1)   //平移将球铲到5区域
                                    {
                                        decisions[1].TCode = 8;
                                        decisions[1].VCode = 11;
                                    }
                                    break;
                            }
                        }
                        else
                            flag_2[1] = 1;
                    }
                    if (flag_2[1] == 1)   //顶1号球
                    {
                        if (Field(ball1) == 5 || Field(ball1) == 9)
                        {
                            switch (Field(ball1))
                            {
                                case 5:
                                    switch (flag_1[4])
                                    {
                                        case 0:
                                            Helpers.PoseToPose(ref decisions[1], My_fish2, q1, (float)(Math.PI * 3 / 4), 10.0f, 100, 100, ref time2);    //往左下角顶
                                            if (distance(fish2_Position, q1) < 100)
                                                flag_1[4]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, b1, (float)(Math.PI * w1 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                            if (ball1.X<-1150)
                                                flag_1[4]++;
                                            break;
                                        default: break;
                                    }
                                    break;
                                case 9:
                                    switch (flag_1[5])
                                    {
                                        case 0:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftBottomCorner, (float)(Math.PI / 3), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightGoal_LeftBottomCorner) < 100)
                                                flag_1[5]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, temp2, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(temp2, fish2_head) < 100)
                                                flag_1[5]++;
                                            break;
                                        case 2:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_BotttomCorner, (float)(Math.PI / 2), 5, 10, 200, 10, 8, 15, 100, true);
                                            if (distance(fish2_head, RightCourt_BotttomCorner) < 20)
                                                flag_1[5]++;
                                            break;
                                        case 3:
                                            decisions[1].TCode = 8;
                                            decisions[1].VCode = 11;
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                    if(flag_2[2]==1)      //顶2号球
                    {
                        if (Field(ball2) == 5 || Field(ball2) == 9)
                        {
                            switch(Field(ball2))
                            {
                                case 5:
                                    switch(flag_1[6])
                                    {
                                        case 0:
                                            Helpers.PoseToPose(ref decisions[1], My_fish2, q2, (float)(Math.PI * 3 / 4), 10.0f, 100, 100, ref time2);
                                            if (distance(q2, fish2_Position) < 100)
                                                flag_1[6]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, b2, (float)(Math.PI * w2 / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                            if (ball2.X < -1150)
                                                flag_1[6]++;
                                            break;
                                    }
                                    break;
                                case 9:
                                    switch(flag_1[7])
                                    {
                                        case 0:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftBottomCorner, (float)(Math.PI / 3), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(fish2_head, RightGoal_LeftBottomCorner) < 100)
                                                flag_1[7]++;
                                            break;
                                        case 1:
                                            Helpers.Dribble(ref decisions[1], My_fish2, temp2, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                            if (distance(temp2, fish2_head) < 100)
                                                flag_1[7]++;
                                            break;
                                        case 2:
                                            Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_BotttomCorner, (float)(Math.PI / 2), 5, 10, 200, 10, 8, 15, 100, true);
                                            if (distance(fish2_head, RightCourt_BotttomCorner) < 20)
                                                flag_1[7]++;
                                            break;
                                        case 3:
                                            decisions[1].TCode = 8;
                                            decisions[1].VCode = 11;
                                            break;
                                    }
                                    break;
                            }
                        }
                        else
                            flag_1[2] = 1;
                    }
                }
                else
                    flag_1[2] = 1;
            }
            if(flag_1[2]==1)   //只有一个三分球在2号或者9号区域
            {
                if(Field(ball0)==2 || Field(ball1)==2 ||Field(ball2)==2)
                {
                    switch(Field(fish2_Position))
                    {
                        case 2:
                            switch (flag_1[8])
                            {
                                case 0:
                                    Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftTopCorner, (float)(Math.PI / -3), 5, 10, 200, 14, 13, 15, 100, true);
                                    if (distance(fish2_head, RightGoal_LeftTopCorner) < 100)
                                        flag_1[8]++;
                                    break;
                                case 1:
                                    Helpers.Dribble(ref decisions[1], My_fish2, temp1, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                    if (distance(temp1, fish2_head) < 100)
                                        flag_1[8]++;
                                    break;
                                case 2:
                                    Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_TopCorner, (float)(Math.PI / -2), 5, 10, 200, 14, 13, 15, 100, true);
                                    if (distance(fish2_head, RightCourt_TopCorner) < 20)
                                        flag_1[8]++;
                                    break;
                                case 3:
                                    decisions[1].VCode = 11;
                                    decisions[1].TCode = 6;
                                    break;
                                default: break;
                            }
                            break;
                        case 5:
                            switch(flag_1[9])
                            {
                                case 0:
                                    if(Field(ball0)==5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, p0, (float)Math.PI * 3 / -4, 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, p0) < 100)
                                        { flag_2_4[0] = 1; flag_1[9]++; }
                                    }
                                    else if(Field(ball1)==5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, p1, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, p1) < 100)
                                        { flag_2_4[1] = 1; flag_1[9]++; }
                                    }
                                    else if(Field(ball2)==5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, p2, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, p2) < 100)
                                        { flag_2_4[2] = 1; flag_1[9]++; }
                                    }
                                    break;
                                case 1:
                                    if(flag_2_4[0]==1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, a0, (float)(r0 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball0, LeftCourt_TopCorner) < 2 * R) flag_1[9]++;
                                    }
                                    if(flag_2_4[1]==1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, a1, (float)(r1 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball1, LeftCourt_TopCorner) < 2 * R) flag_1[9]++;
                                    }
                                    if(flag_2_4[2]==1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, a2, (float)(r2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball2, LeftCourt_TopCorner) < 2 * R) flag_1[9]++;
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else if(Field(ball0)==9 || Field(ball1)==9 || Field(ball2)==9)
                {
                    switch (Field(fish2_Position))
                    {
                        case 5:
                            switch (flag_1[10])
                            {
                                case 0:
                                    if (Field(ball0) == 5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, q0, (float)Math.PI * 3 / -4, 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, q0) < 100)
                                        { flag_2_5[0] = 1; flag_1[10]++; }
                                    }
                                    else if (Field(ball1) == 5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, q1, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, q1) < 100)
                                        { flag_2_5[1] = 1; flag_1[10]++; }
                                    }
                                    else if (Field(ball2) == 5)
                                    {
                                        Helpers.PoseToPose(ref decisions[1], My_fish2, q2, (float)(Math.PI * 3 / -4), 10.0f, 100, 100, ref time4);
                                        if (distance(fish2_Position, q2) < 100)
                                        { flag_2_5[2] = 1; flag_1[10]++; }
                                    }
                                    break;
                                case 1:
                                    if (flag_2_5[0] == 1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, b0, (float)(w0 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball0, LeftCourt_TopCorner) < 2 * R) flag_1[10]++;
                                    }
                                    if (flag_2_5[1] == 1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, b1, (float)(w1 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball1, LeftCourt_TopCorner) < 2 * R) flag_1[10]++;
                                    }
                                    if (flag_2_5[2] == 1)
                                    {
                                        Helpers.Dribble(ref decisions[1], My_fish2, b2, (float)(w2 * Math.PI / 180), 2, 3, 200, 14, 12, 15, 100, true);
                                        if (distance(ball2, LeftCourt_TopCorner) < 2 * R) flag_1[10]++;
                                    }
                                    break;
                            }
                            break;
                        case 9:
                            switch (flag_1[11])
                            {
                                case 0:
                                    Helpers.Dribble(ref decisions[1], My_fish2, RightGoal_LeftBottomCorner, (float)(Math.PI / 3), 5, 10, 200, 14, 13, 15, 100, true);
                                    if (distance(fish2_head, RightGoal_LeftBottomCorner) < 100)
                                        flag_1[11]++;
                                    break;
                                case 1:
                                    Helpers.Dribble(ref decisions[1], My_fish2, temp2, (float)(Math.PI), 5, 10, 200, 14, 13, 15, 100, true);
                                    if (distance(temp2, fish2_head) < 100)
                                        flag_1[11]++;
                                    break;
                                case 2:
                                    Helpers.Dribble(ref decisions[1], My_fish2, RightCourt_BotttomCorner, (float)(Math.PI / 2), 5, 10, 200, 10, 8, 15, 100, true);
                                    if (distance(fish2_head, RightCourt_BotttomCorner) < 20)
                                        flag_1[11]++;
                                    break;
                                case 3:
                                    decisions[1].TCode = 8;
                                    decisions[1].VCode = 11;
                                    break;
                            }
                            break;
                    }
                }
            }
            /*#region   在别人的球门铲球
            switch(Field(fish2_Position))
            {
                case 2:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 9:
                    break;
            }
            #endregion*/

        }

        public static void Moving_RightSecondhalf(Mission mission, int teamId, ref Decision[] decisions)
        {
            #region 自定义数据的定义

            xna.Vector3 Ball0 = new xna.Vector3(ball0.X, 0, ball0.Z - 3 * R);
            xna.Vector3 Ball1 = new xna.Vector3(ball1.X, 0, ball1.Z - 3 * R);
            xna.Vector3 Ball2 = new xna.Vector3(ball2.X, 0, ball2.Z + 3 * R);

            My_fish1 = mission.TeamsRef[teamId].Fishes[0];
            My_fish2 = mission.TeamsRef[teamId].Fishes[1];

            MyScore = mission.TeamsRef[teamId].Para.Score;

            fish1_head = mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0];
            fish1_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[0].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[0].PolygonVertices[4].Z) / 2);
            fish1_Position = mission.TeamsRef[teamId].Fishes[0].PositionMm;

            fish2_head = mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0];
            fish2_body = new xna.Vector3((mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].X + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].X) / 2, 0, (mission.TeamsRef[teamId].Fishes[1].PolygonVertices[0].Z + mission.TeamsRef[teamId].Fishes[1].PolygonVertices[3].Z) / 2);
            fish2_Position = mission.TeamsRef[teamId].Fishes[1].PositionMm;
            efish1_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[0];
            efish1_Position = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PositionMm;
            efish2_head = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].PolygonVertices[1];
            efish2_Position = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].PositionMm;
            efish1_BodyDirectionRad = mission.TeamsRef[(1 + teamId) % 2].Fishes[0].BodyDirectionRad;
            efish2_BodyDirectionRad = mission.TeamsRef[(1 + teamId) % 2].Fishes[1].BodyDirectionRad;

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

            Ball0_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Right_Status"]);
            Ball1_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Right_Status"]);
            Ball2_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Right_Status"]);
            Ball3_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Right_Status"]);
            Ball4_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Right_Status"]);
            Ball5_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Right_Status"]);
            Ball6_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Right_Status"]);
            Ball7_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Right_Status"]);
            Ball8_Right = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Right_Status"]);

            Ball0_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_0_Left_Status"]);
            Ball1_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_1_Left_Status"]);
            Ball2_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_2_Left_Status"]);
            Ball3_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_3_Left_Status"]);
            Ball4_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_4_Left_Status"]);
            Ball5_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_5_Left_Status"]);
            Ball6_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_6_Left_Status"]);
            Ball7_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_7_Left_Status"]);
            Ball8_Left = Convert.ToInt32(mission.HtMissionVariables["Ball_8_Left_Status"]);

            mission.CommonPara.MsPerCycle = CycleTime;

            RemainCycle = mission.CommonPara.RemainingCycles;
            MsPerCycle = mission.CommonPara.MsPerCycle;

            #endregion

            #region 临时点
            xna.Vector3 p = new xna.Vector3(1100, 0, -700);
            xna.Vector3 q = new xna.Vector3(1100, 0, 700);
            double r0 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(RightCourt_TopCorner - ball2));
            xna.Vector3 a0 = new xna.Vector3(ball2.X - R * (float)Math.Cos(r0), 0, ball2.Z - R * (float)Math.Sin(r0));
            xna.Vector3 p0 = new xna.Vector3(ball2.X - 3 * R, 0, ball2.Z + 3 * R);

            double r1 = xna.MathHelper.ToRadians((float)Helpers.GetAngleDegree(RightCourt_TopCorner - ball8));
            xna.Vector3 a1 = new xna.Vector3(ball8.X - R * (float)Math.Cos(r1), 0, ball8.Z - R * (float)Math.Sin(r1));
            xna.Vector3 p1 = new xna.Vector3(ball8.X - 3 * R, 0, ball8.Z + 3 * R);


            #endregion


            #region  如果我的分数高于9
            if (MyScore >= 9)  //防守模式
            {
                Helpers.Dribble(ref decisions[1], My_fish2, efish2_Position, (float)Math.PI / 2 - efish2_BodyDirectionRad, 2, 3, 200, 14, 13, 15, 100, true);
            }
            #endregion

            #region  如果我的分数低于9
            else
            {
                #region  根据球的位置判断自己的策略
                if (ball0.X > 1100)   //判断球0的位置
                    flag = flag + 3;
                else if (ball0.X > -800)
                {
                    flag_2_2[0] = 1;
                    free = free + 3;
                }
                else
                    eflag = eflag + 3;

                if (ball1.X > 1100)   //判断球1的位置
                    flag = flag + 3;
                else if (ball1.X > -800)
                {
                    flag_2_2[1] = 1;
                    free = free + 3;
                }
                else
                    eflag = eflag + 3;

                if (ball2.X > 1100)   //判断球2的位置
                    flag = flag + 3;
                else if (ball2.X > -800)
                {
                    flag_2_2[2] = 1;
                    free = free + 3;
                }
                else
                    eflag = eflag + 3;


                if (ball7.X > 1100)   //判断球7的位置
                    flag = flag + 2;
                else if (ball7.X > -800)
                {
                    flag_2_2[3] = 1;
                    free = free + 3;
                }
                else
                    eflag = eflag + 3;

                if (ball8.X > 1100)    //判断球8的位置
                    flag = flag + 2;
                else if (ball8.X > -800)
                {
                    flag_2_2[4] = 1;
                    free = free + 3;
                }
                else
                    eflag = eflag + 3;

                if (ball3.X > 1000)
                    flag = flag + 1;
                if (ball6.X > 1000)
                    flag = flag + 1;
                if (ball4.X < -800)
                    eflag = eflag + 1;
                if (ball5.X < -800)
                    eflag = eflag + 1;
                #endregion
                if (MyScore > EnemyScore)  //顶鱼,干扰模式 或者  继续得分
                {

                }
                else if (MyScore == EnemyScore)
                {


                }
                else if (MyScore < EnemyScore)
                {
                    if (flag >= 0 && flag < 6)
                    {
                        if (free > eflag)
                        {
                            flag_2[1] = 1;
                        }
                    }
                    else if (flag >= 6 && flag < 10)
                    {
                        flag_2[0] = 1;
                    }
                    else   //继续顶球入球门
                    {
                        flag_2[2] = 1;
                    }
                }
            }
            if (flag_2[0] == 1)
            {
                if (flag_2_2[2] == 1)  //鱼1优先抢2号球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p0, (float)(Math.PI / -4), 10.0f, 100, 100, ref time1);
                    if (distance(fish2_Position, p0) < 100)
                        flag_2_4[0] = 1;
                }
                if (flag_2_4[0] == 1)
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, a0, (float)(Math.PI * r0), 2, 3, 200, 14, 13, 15, 100, true);
                    if (distance(ball2, RightCourt_TopCorner) < 2 * R)
                        flag_2_4[1] = 1;
                }
                if (flag_2_2[2] == 0 && flag_2_2[4] == 1)   //鱼1抢8号球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p1, (float)(Math.PI / -4), 10.0f, 100, 100, ref time1);
                    if (distance(fish2_Position, p1) < 100)
                        flag_2_4[2] = 1;
                }
                if (flag_2_4[2] == 1)
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, a1, (float)(Math.PI / -4), 2, 3, 200, 14, 13, 15, 100, true);
                    if (distance(ball8, RightCourt_TopCorner) < 2 * R)
                        flag_2[2] = 1;
                }
            }
            if (flag_2[1] == 1)
            {
                if (flag_2_2[2] == 1)  //鱼1优先抢2号球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p0, (float)(Math.PI / -4), 10.0f, 100, 100, ref time2);
                    if (distance(fish2_Position, p0) < 100)
                        flag_2_3[0] = 1;
                }
                if (flag_2_3[0] == 1)
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, a0, (float)(Math.PI * r0), 2, 3, 200, 14, 13, 15, 100, true);
                    if (distance(ball2, RightCourt_TopCorner) < 2 * R)
                        flag_2_3[1] = 1;
                }
                if (flag_2_2[4] == 1 && flag_2_3[1] == 1)   //如果2号球不在范围内,抢8号球
                {
                    Helpers.PoseToPose(ref decisions[1], My_fish2, p1, (float)(Math.PI / -4), 10.0f, 100, 100, ref time3);
                    if (distance(fish2_Position, p1) < 100)
                        flag_2_3[2] = 1;
                }

                if (flag_2_3[2] == 1)
                {
                    Helpers.Dribble(ref decisions[1], My_fish2, a1, (float)(Math.PI * r1), 2, 3, 200, 14, 13, 15, 100, true);
                    if (distance(ball8, RightCourt_TopCorner) < 2 * R)
                        flag_2[2] = 1;
                }
            }

            #endregion







        }
    }
}
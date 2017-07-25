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
            float deltaAngle = a - b;
            if (deltaAngle > Math.PI) deltaAngle -= (float)(2 * Math.PI);
            if (deltaAngle > Math.PI) deltaAngle += (float)(2 * Math.PI);

            if (deltaAngle > 0.2) return 1;//a在b右边
            else if (deltaAngle < -0.2) return -1; //a在b左边
            else return 0;
        }
        public float CalAngle(Vector3 presentPoint,Vector3 destPoint)
        {
            float angle;
            angle = Helpers.GetAngleDegree(destPoint - presentPoint);
            angle = angle / 180 * (float)Math.PI;
            if (angle > Math.PI) angle -= (float)(2 * Math.PI);
            if (angle > Math.PI) angle += (float)(2 * Math.PI);
            return angle;
        }
        public Vector3 CalPointOnBall(Vector3 ball,float targetDirection)
        {
            double x = ball.X - Math.Cos(targetDirection) *80;
            double z = ball.Z - Math.Sin(targetDirection) *80;
            Vector3 point = new Vector3((float)x, 0, (float)z);
            return point;
        }
        public float GetVectorDistance(xna.Vector3 a, xna.Vector3 b)
        {
            return (float)Math.Sqrt((Math.Pow((a.X - b.X), 2d) + Math.Pow((a.Z - b.Z), 2d)));
        }
        public void OneFishGetScore(RoboFish fish,int leftOrRight,Vector3 ball,ref Decision decision)//leftOrRight参数：1为left，2为right
        {
            Vector3 fishLocation = fish.PositionMm;
            float fishDirection = fish.BodyDirectionRad;
            Vector3 targetPoint;
            float targetDirection;
            if (leftOrRight == 1)//自己球门在左边
            {
                Vector3 upPoint = new Vector3(-1000, 0, -500);
                Vector3 bottomPoint = new Vector3(-1000, 0, 500);
                Vector3 upTempPoint = new Vector3(-1300, 0, -700);
                Vector3 bottomTempPoint = new Vector3(-1300, 0, 700);
                if (ball.X > -1000) //球在门外
                {
                    if (ball.Z > 0)//距离上面的点近一点
                    {
                        targetDirection = CalAngle(ball, upTempPoint);
                    }
                    else
                    {
                        targetDirection = CalAngle(ball, bottomTempPoint);
                    }
                    targetPoint = CalPointOnBall(ball, targetDirection);
                    if (GetVectorDistance(ball, fishLocation) > 150)//快速游到目标点
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 20f, 30f, 200, 14, 12, 15, 100, true);
                    else
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                }
                else if (ball.X <= 1250)
                {
                    if (ball.Z < -300)
                    {
                        targetDirection = CalAngle(ball, bottomPoint);
                        targetPoint = CalPointOnBall(ball, targetDirection);
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                    }
                    else if (ball.Z > 300)
                    {
                        targetDirection = CalAngle(ball, upPoint);
                        targetPoint = CalPointOnBall(ball, targetDirection);
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                    }
                    else
                    {
                        if (fishDirection < 0)
                        {
                            targetDirection = CalAngle(ball, upPoint);
                            targetPoint = CalPointOnBall(ball, targetDirection);
                            Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 7, 5, 15, 100, true);
                        }
                        else
                        {
                            targetDirection = CalAngle(ball, bottomPoint);
                            targetPoint = CalPointOnBall(ball, targetDirection);
                            Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 7, 5, 15, 100, true);
                        }
                    }

                }
                else if (ball.X <= -1000 && ball.X > -1250) 
                {
                    if (ball.Z < -500)//卡在球门上侧的情况
                    {
                        targetDirection = CalAngle(ball, upTempPoint);
                        targetPoint = CalPointOnBall(ball, targetDirection);
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                    }
                    else if (ball.Z > 500)//卡在球门上侧的情况
                    {
                        targetDirection = CalAngle(ball, bottomTempPoint);
                        targetPoint = CalPointOnBall(ball, targetDirection);
                        Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                    }
                    else//球门区域内
                    {
                        if (fishLocation.Z > ball.Z + 60)//鱼在球下面
                        {
                            targetDirection = -(float)Math.PI * 0.61f;
                            targetPoint = new Vector3(ball.X - 50, 0, ball.Z - 20);
                            if (GetVectorDistance(fishLocation, targetPoint) > 50 && IsDirectionRight(fishDirection, targetDirection) != 0) 
                                Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                            else
                            {
                                decision.TCode = 12;
                                decision.VCode = 1;
                            }
                        }
                        else//鱼在球上面
                        {
                            targetDirection = (float)Math.PI * 0.61f;
                            targetPoint = new Vector3(ball.X - 50, 0, ball.Z + 20);
                            if (GetVectorDistance(fishLocation, targetPoint) > 50 && IsDirectionRight(fishDirection, targetDirection) != 0)
                                Helpers.Dribble(ref decision, fish, targetPoint, targetDirection, 2f, 3f, 80, 8, 6, 15, 100, true);
                            else
                            {
                                decision.TCode = 3;
                                decision.VCode = 1;
                            }
                        }
                    }
                }
            }
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
            RoboFish fish1 = mission.TeamsRef[teamId].Fishes[0];
            Vector3 ball1 = mission.EnvRef.Balls[0].PositionMm;
            OneFishGetScore(fish1, 1, ball1, ref decisions[0]);

        
            return decisions;
        }
    }
}

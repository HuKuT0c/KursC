﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Repeat
{
     public class Emitter
    {
        public List<Particle> particles = new List<Particle>();
        public int count=0;

     //   public int MousePositionX;
    //    public int MousePositionY;
        public float GravitationX = 0;
        public float GravitationY = 1;       
        public List<IImpactPoint> impactPoints = new List<IImpactPoint>();
        public int ParticlesCount = 1500;
        //---
        public int X; // координата X центра эмиттера
        public int Y; // соответствующая координата Y 
        public int Direction = 0; // вектор направления в градусах куда сыпет эмиттер
        public int Spreading = 360; // разброс частиц относительно Direction
        public int SpeedMin = 1; // начальная минимальная скорость движения частицы
        public int SpeedMax = 10; // начальная максимальная скорость движения частицы
        public int RadiusMin = 2; // минимальный радиус частицы
        public int RadiusMax = 10; // максимальный радиус частицы
        public int LifeMin = 20; // минимальное время жизни частицы
        public int LifeMax = 100; // максимальное время жизни частицы

        public int ParticlesPerTick = 1;

        public Color ColorFrom = Color.White; // начальный цвет частицы
        public Color ColorTo = Color.FromArgb(0, Color.Black); // конечный цвет частиц

        public void UpdateState()
        {
            int particlesToCreate = ParticlesPerTick;
            count = 0;
            foreach (var particle in particles)
            {
                if (particle.Life <= 0) // если частицы умерла
                {
                    /* 
                     * то проверяем надо ли создать частицу
                     */
                    count--;
                    if (particlesToCreate > 0)
                    {
                        /* у нас как сброс частицы равносилен созданию частицы */
                        particlesToCreate -= 1; // поэтому уменьшаем счётчик созданных частиц на 1
                        ResetParticle(particle);
                        count++;
                    }
                }
                else
                {
                    // так как теперь мы храним вектор скорости в явном виде и его не надо пересчитывать
                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;

                    particle.Life -= 1;
                    foreach (var point in impactPoints)
                    {
                        point.ImpactParticle(particle);
                    }

                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                    //count++;
                }
                count++;
            }

            while (particlesToCreate >= 1)
            {
                particlesToCreate -= 1;
                var particle = CreateParticle();
                ResetParticle(particle);
                particles.Add(particle);
            //    count++;
            }

          //  count = particles.Count;
        }

        public void Render(Graphics g)
        {
           
            foreach (var particle in particles)
            {
                particle.Draw(g);
            }

            foreach (var point in impactPoints) 
            {              
                point.Render(g); 
            }

        }

        public virtual void ResetParticle(Particle particle)
        {
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);

            particle.X = X;
            particle.Y = Y;

          //  /*
            var direction = Direction
                + (double)Particle.rand.Next(Spreading)
                - Spreading / 2;

            var speed = Particle.rand.Next(SpeedMin, SpeedMax);
            
            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);
            //   */

            /*
               particle.direction = Direction+ Particle.rand.Next(Spreading)- Spreading / 2;
               particle.speed = Particle.rand.Next(SpeedMin, SpeedMax);

               particle.SpeedX = (float)(Math.Cos(particle.direction / 180 * Math.PI) * particle.speed);
               particle.SpeedY = -(float)(Math.Sin(particle.direction / 180 * Math.PI) * particle.speed);
            */
            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);
         
            //--
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;
        }

        public virtual Particle CreateParticle()
        {
            //  var particle = new ParticleColorful();
            var particle = new Particle();
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;

            return particle;
        }
    }


    //snow
    public class TopEmitter : Emitter
    {
        public int Width; // длина экрана

        public override void ResetParticle(Particle particle)
        {
            base.ResetParticle(particle); // вызываем базовый сброс частицы, там жизнь переопределяется и все такое

            // а теперь тут уже подкручиваем параметры движения
            particle.X = Particle.rand.Next(Width); // позиция X -- произвольная точка от 0 до Width
            particle.Y = 0;  // ноль -- это верх экрана 

            particle.SpeedY = 1; // падаем вниз по умолчанию
            particle.SpeedX = Particle.rand.Next(-2, 2); // разброс влево и вправа у частиц 

            particle.FromColor = Color.White;
            particle.ToColor = Color.FromArgb(0, Color.Black);

            /*
            if  (particle is ParticleColorful) 
            {               
                particle.ChangeColorToWhite();
            }
            */
        }
    }
}

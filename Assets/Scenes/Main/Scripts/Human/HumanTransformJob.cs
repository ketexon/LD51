﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System.Runtime.InteropServices;

namespace LD51
{
    [System.Serializable]
    public struct HumanParameters
    {
        public float Velocity;

        public float MinMoveTime;
        public float MaxMoveTime;

        public float MinWaitTime;
        public float MaxWaitTime;
    }

    [System.Serializable]
    public struct HumanInstanceData
    {
        // RNG
        public Random Random;
        // Time to move to new state (waiting/moving)
        public float EndStateTime;
        // True if human should be sitting still
        public bool Waiting;
        // Unit vector of direction to move
        public float2 MoveDirection;
    }

    [BurstCompile(CompileSynchronously = true)]
    struct HumanTransformJob : IJobParallelForTransform
    {
        [StructLayout(LayoutKind.Explicit)]
        struct Reinterpret
        {
            [FieldOffset(0)]
            public int IntValue;
            [FieldOffset(0)]
            public uint UIntValue;
            [FieldOffset(0)]
            public float FloatValue;

            public static uint FloatToUInt(float floatValue)
            {
                return new Reinterpret { FloatValue = floatValue }.UIntValue;
            }

            public static uint IntToUInt(float floatValue)
            {
                return new Reinterpret { FloatValue = floatValue }.UIntValue;
            }
        }

        public HumanParameters HumanParameters;
        public NativeArray<HumanInstanceData> HumanInstances;
        public float Time;
        public float DeltaTime;

        public void Execute(int idx, TransformAccess transform)
        {
            bool modified = false;
            var human = HumanInstances[idx];
            if (!human.Waiting)
            {
                if(Time > human.EndStateTime)
                {
                    modified = true;
                    // Seed with time reinterpreted as float + index

                    human.Waiting = true;
                    human.EndStateTime
                        += human.Random.NextFloat(
                            HumanParameters.MinWaitTime,
                            HumanParameters.MaxWaitTime
                        );
                }
                else
                {
                    transform.position
                        += DeltaTime * HumanParameters.Velocity * new Vector3(
                            human.MoveDirection.x,
                            human.MoveDirection.y,
                            0
                        );
                }
            }
            else // Waiting
            {
                if(Time > human.EndStateTime)
                {
                    human.Waiting = false;
                    human.EndStateTime
                        += human.Random.NextFloat(
                            HumanParameters.MinMoveTime,
                            HumanParameters.MaxMoveTime
                        );
                    var theta = human.Random.NextFloat(
                        0,
                        2 * math.PI
                    );
                    human.MoveDirection = new float2
                    {
                        x = math.cos(theta),
                        y = math.sin(theta),
                    };
                    modified = true;
                }
            }

            if (modified)
            {
                HumanInstances[idx] = human;
            }
        }
    }
}
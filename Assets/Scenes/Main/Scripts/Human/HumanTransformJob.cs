using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System.Runtime.InteropServices;

namespace LD51
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Reinterpret
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

        public static uint IntToUInt(int floatValue)
        {
            return new Reinterpret { FloatValue = floatValue }.UIntValue;
        }
    }

    [System.Serializable]
    public struct HumanParameters
    {
        public float Velocity;

        public float MinMoveTime;
        public float MaxMoveTime;

        public float MinWaitTime;
        public float MaxWaitTime;

        public Vector2 Range;
        public Vector2 MinSpawnRange;
    }

    [System.Serializable]
    public struct HumanInstanceData
    {
        // Time to move to new state (waiting/moving)
        public float EndStateTime;
        // True if human should be sitting still
        public bool Waiting;
        // Unit vector of direction to move
        public float2 MoveDirection;
    }

    [BurstCompile(
        CompileSynchronously = true,
        OptimizeFor = OptimizeFor.Performance
    )]
    struct HumanTransformJob : IJobParallelForTransform
    {
        public Random Random;
        public HumanParameters HumanParameters;
        public NativeArray<HumanInstanceData> HumanInstances;
        public Vector2 PlayerPosition;
        public float Time;
        public float DeltaTime;

        public void Execute(int idx, TransformAccess transform)
        {
            bool modified = false;
            var human = HumanInstances[idx];
            if (!human.Waiting)
            {
                if (Time > human.EndStateTime)
                {
                    modified = true;
                    // Seed with time reinterpreted as float + index

                    human.Waiting = true;
                    human.EndStateTime
                        = Time + Random.NextFloat(
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
                    EnforceRange(transform);
                }
            }
            else // Waiting
            {
                if (Time > human.EndStateTime)
                {
                    human.Waiting = false;
                    human.EndStateTime
                        = Time + Random.NextFloat(
                            HumanParameters.MinMoveTime,
                            HumanParameters.MaxMoveTime
                        );
                    var theta = Random.NextFloat(
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

        // This function checks if position is outside
        // HumanParameters.Range relative to PlayerPosition
        // and moves the human to a random position on the other side
        // of the player between MinSpawnRange
        // and Range
        void EnforceRange(TransformAccess transform)
        {
            Vector2 delta = (Vector2)transform.position - PlayerPosition;
            Debug.Log(string.Format(
                "Delta: ({0}, {1})",
                delta.x,
                delta.y
            ));
            if (math.abs(delta.x) > HumanParameters.Range.x)
            {
                transform.position
                    -= Vector3.right * (
                        delta.x // move to origin
                        + math.sign(delta.x)
                        * Random.NextFloat(
                            HumanParameters.MinSpawnRange.x,
                            HumanParameters.Range.x
                        )
                    );
            }
            if (math.abs(delta.y) > HumanParameters.Range.y)
            {
                transform.position
                    -= Vector3.up * (
                        delta.y // move to origin
                        + math.sign(delta.y)
                        * Random.NextFloat(
                            HumanParameters.MinSpawnRange.y,
                            HumanParameters.Range.y
                        )
                    );
            }
        }
    }
}

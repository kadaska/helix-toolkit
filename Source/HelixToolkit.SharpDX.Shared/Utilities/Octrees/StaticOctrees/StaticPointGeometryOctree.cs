﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define DEBUG
using SharpDX;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// Static octree for points
    /// </summary>
    public class StaticPointGeometryOctree : StaticOctree<int>
    {
        private static readonly Vector3 BoundOffset = new Vector3(0.001f);
        protected readonly IList<Vector3> Positions;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="parameter"></param>
        /// <param name="stackCache"></param>
        public StaticPointGeometryOctree(IList<Vector3> positions,
            OctreeBuildParameter parameter, Stack<KeyValuePair<int, IDynamicOctree[]>> stackCache = null)
               : base(parameter)
        {
            Positions = positions;
        }

        protected override BoundingBox GetBoundingBoxFromItem(int item)
        {
            return new BoundingBox(Positions[item] - BoundOffset, Positions[item] + BoundOffset);
        }

        protected override BoundingBox GetMaxBound()
        {
            return BoundingBoxExtensions.FromPoints(Positions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override bool IsContains(BoundingBox source, BoundingBox target, int obj)
        {
            return source.Contains(Positions[obj]) != ContainmentType.Disjoint;
        }

        protected override int[] GetObjects()
        {
            var objects = new int[Positions.Count];
            for (int i = 0; i < Positions.Count; ++i)
            {
                objects[i] = i;
            }
            return objects;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="octant"></param>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="modelMatrix"></param>
        /// <param name="rayWS"></param>
        /// <param name="rayModel"></param>
        /// <param name="hits"></param>
        /// <param name="isIntersect"></param>
        /// <param name="hitThickness"></param>
        /// <returns></returns>
        protected override bool HitTestCurrentNodeExcludeChild(Octant octant, IRenderContext context, object model, Matrix modelMatrix, ref Ray rayWS, ref Ray rayModel, ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness)
        {
            isIntersect = false;
            if (!octant.IsBuilt || context == null)
            {
                return false;
            }
            var isHit = false;
            var bound = octant.Bound;
            if (rayModel.Intersects(ref bound))
            {
                isIntersect = true;
                if (octant.Count == 0)
                {
                    return false;
                }
                var result = new HitTestResult();
                result.Distance = double.MaxValue;
                var svpm = context.ScreenViewProjectionMatrix;
                var smvpm = modelMatrix * svpm;
                var clickPoint4 = new Vector4(rayWS.Position + rayWS.Direction, 1);
                var pos4 = new Vector4(rayWS.Position, 1);
                Vector4.Transform(ref clickPoint4, ref svpm, out clickPoint4);
                Vector4.Transform(ref pos4, ref svpm, out pos4);
                var clickPoint = clickPoint4.ToVector3();

                isIntersect = true;
                var dist = hitThickness;
                for (int i = octant.Start; i < octant.End; ++i)
                {
                    var v0 = Positions[Objects[i]];
                    var p0 = Vector3.TransformCoordinate(v0, smvpm);
                    var pv = p0 - clickPoint;
                    var d = pv.Length();
                    if (d < dist) // If d is NaN, the condition is false.
                    {
                        dist = d;
                        result.IsValid = true;
                        result.ModelHit = model;
                        var px = Vector3.TransformCoordinate(v0, modelMatrix);
                        result.PointHit = px;
                        result.Distance = (rayWS.Position - px).Length();
                        result.Tag = Objects[i];
                        isHit = true;
                    }
                }

                if (isHit)
                {
                    isHit = false;
                    if (hits.Count > 0)
                    {
                        if (hits[0].Distance > result.Distance)
                        {
                            hits[0] = result;
                            isHit = true;
                        }
                    }
                    else
                    {
                        hits.Add(result);
                        isHit = true;
                    }
                }
            }

            return isHit;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="octant"></param>
        /// <param name="context"></param>
        /// <param name="sphere"></param>
        /// <param name="result"></param>
        /// <param name="isIntersect"></param>
        /// <returns></returns>
        protected override bool FindNearestPointBySphereExcludeChild(Octant octant, IRenderContext context,
            ref BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect)
        {
            bool isHit = false;
            var resultTemp = new HitTestResult();
            resultTemp.Distance = float.MaxValue;
            var containment = octant.Bound.Contains(ref sphere);
            if (containment != ContainmentType.Disjoint)
            {
                isIntersect = true;
                for (int i = octant.Start; i < octant.End; ++i)
                {
                    var p = Positions[Objects[i]];
                    containment = sphere.Contains(ref p);
                    if (containment != ContainmentType.Disjoint)
                    {
                        var d = (p - sphere.Center).Length();
                        if (resultTemp.Distance > d)
                        {
                            resultTemp.Distance = d;
                            resultTemp.IsValid = true;
                            resultTemp.PointHit = p;
                            resultTemp.Tag = Objects[i];
                            isHit = true;
                        }
                    }
                }
                if (isHit)
                {
                    isHit = false;
                    if (result.Count > 0)
                    {
                        if (result[0].Distance > resultTemp.Distance)
                        {
                            result[0] = resultTemp;
                            isHit = true;
                        }
                    }
                    else
                    {
                        result.Add(resultTemp);
                        isHit = true;
                    }
                }
            }
            else
            {
                isIntersect = false;
            }
            return isHit;
        }
    }
}

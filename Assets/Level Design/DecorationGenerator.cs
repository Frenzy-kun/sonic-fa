using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationGenerator : MonoBehaviour
{
#pragma warning disable IDE1006 // Estilos de nombres
    public DecorationGeneratorProfile mDecorationProfile;
#pragma warning restore IDE1006 // Estilos de nombres

    public int density = 3;

    public float Zpush = 64f;
    public int maxLayers = 1;

    public float yOffset = 10f;
    public float xMargin = 10f;
    public float mBgOffset= 10f;
    public float yOffsetMultiplier = 10f;

    public Vector2 mScaleMultiplier = Vector2.one;

    public bool mClearBeforeRegenerate = true;
    public string mGeneratedLayerMask = "Decoration Layer";


    private void Update()
    {
        Collider2D collider = this.GetComponent<Collider2D>();

        Bounds cb = collider.bounds;
        
    }

    public void Regenerate()
    {
        Collider2D collider = this.GetComponent<Collider2D>();
        
        if (!collider)
        {
            Debug.LogError("You are trying to generate decorations when there's no collider");
        }
        else
        {
            if (!this.mDecorationProfile)
            {
                Debug.LogError("You need to add a DecorationGeneratorProfile to generate decorations. Use LevelEditor/DecorationGeneratorProfile to create one");
            }
            else
            {
                this.Generate(collider);
            }
        }
    }

    private void Generate(Collider2D collider)
    {
        if (mClearBeforeRegenerate)
            this.ClearAll();

        int layermask = LayerMask.NameToLayer(mGeneratedLayerMask);
        int random;
        GameObject deco;
        Vector3 finalPosition;

        Debug.Log(collider.bounds.min);
        Debug.Log(collider.bounds.max);

        for (int i = 1; i < maxLayers; i++)
        {
            deco = Instantiate(this.gameObject, this.transform, false);
            deco.layer = layermask;
            DestroyImmediate(deco.GetComponent<Collider2D>());
            DestroyImmediate(deco.GetComponent<SpriteShapeExtras.LegacyCollider>());
            deco.GetComponent<DecorationGenerator>().ClearAll();
            DestroyImmediate(deco.GetComponent<DecorationGenerator>());
            deco.transform.localPosition = Vector3.forward * (Zpush * i);

            //Duplicate points
            for (int j = 1; j < deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.GetPointCount(); j+=2)
            {
                deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.InsertPointAt(j,
                    (deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.GetPosition(j) + deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.GetPosition(j -1)) * 0.5f
                    );
            }

            for (int j = 0; j < deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.GetPointCount(); j++)
            {
                finalPosition = deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.GetPosition(j);
                finalPosition.x += UnityEngine.Random.RandomRange(-mBgOffset*i, mBgOffset*i);
                finalPosition.y += UnityEngine.Random.RandomRange(0f, mBgOffset + yOffsetMultiplier * i) + yOffsetMultiplier * i;
                deco.GetComponent<UnityEngine.U2D.SpriteShapeController>().spline.SetPosition(j, finalPosition);
            }

            //deco.transform.localScale = Vector3.one * UnityEngine.Random.RandomRange(0.99f, 1.01f);
        }

        int randomLayer;

        for (int i = 0; i < this.density; i++)
        {
            random = UnityEngine.Random.Range(0, this.mDecorationProfile.mDecorations.Count);
            randomLayer = UnityEngine.Random.Range(1, maxLayers);
            deco = Instantiate(this.mDecorationProfile.mDecorations[random], this.transform, false);
            deco.layer = layermask;

            finalPosition = new Vector3(
                UnityEngine.Random.Range(collider.bounds.min.x + xMargin, collider.bounds.max.x - xMargin),
                collider.bounds.max.y + 16f,
                this.transform.position.z + this.Zpush * randomLayer
                );

            RaycastHit2D hit = Physics2D.Raycast(finalPosition, Vector2.down);


            if (hit.collider)
            {
                finalPosition.x = hit.point.x;
                finalPosition.y = hit.point.y;

                finalPosition.y += yOffset + yOffsetMultiplier * randomLayer;

                deco.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * deco.transform.rotation;
            }

            deco.transform.position = finalPosition;

            deco.transform.localScale = Vector3.one * UnityEngine.Random.Range(mScaleMultiplier.x, mScaleMultiplier.y);

        }
    }

    public void ClearAll()
    {
        while (this.transform.childCount > 0)
        {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }
    }
}

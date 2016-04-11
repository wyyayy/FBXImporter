using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ModelImporter
{
    public class Importer
    {
		static FBXImporter fbxImporter = new FBXImporter();

		static Vector3 FBXUpVectorToUnityVector3( FBXGlobalSettings.UpVector up )
		{
			switch( up )
			{
				case FBXGlobalSettings.UpVector.X: return new Vector3( 1.0f, 0.0f, 0.0f );
				case FBXGlobalSettings.UpVector.Y: return new Vector3( 0.0f, 0.0f, 1.0f );
				case FBXGlobalSettings.UpVector.Z: return new Vector3( 0.0f, 1.0f, 0.0f );
			}

			return Vector3.zero;
		}

		static Vector2 FBXVector2ToUnityVector2( FBXVector2 v )
		{
			return new Vector2( v.x, v.y );
		}

		static Vector3 FBXVector3ToUnityVector3Abs( FBXVector3 v )
		{
			return new Vector3( v.x, v.y, v.z );
		}

		static Vector3 FBXVector3ToUnityVector3( FBXVector3 v )
		{
			return new Vector3( -v.x, v.y, v.z );
		}

		static Vector3 FBXNormalToUnityNormal( FBXVector3 v )
		{
			return new Vector3( v.x, -v.y, -v.z );
		}

		static Vector4 FBXVector4ToUnityVector4( FBXVector4 v )
		{
			return new Vector4( v.x, v.y, v.z, v.w );
		}

		static Quaternion FBXVector3ToUnityQuaternion( FBXVector3 v )
		{
			return Quaternion.Euler( new Vector3( v.x, -v.y, -v.z ) );
		}

		static Color FBXColorToUnityColor( FBXColor c )
		{
			return new Color( c.r, c.g, c.b, c.a );
		}

		static Color FBXVector3ToUnityColor( FBXVector3 v )
		{
			return new Color( v.x, v.y, v.z, 1.0f );
		}

		static void GetTrasform( FBXModelPtr model, out Vector3 position, out Quaternion rotation, out Vector3 scale )
		{
			position = FBXVector3ToUnityVector3( model.GetTranslation() );
			rotation = FBXVector3ToUnityQuaternion( model.GetRotation() );
			scale = FBXVector3ToUnityVector3Abs( model.GetScale() );
		}

		static void GetPivotTransform( FBXModelPtr model, out Vector3 position, out Vector3 rotation, out Vector3 scale )
		{
			position = FBXVector3ToUnityVector3( model.GetPivotRotation() ) + FBXVector3ToUnityVector3( model.GetRotationOffset() );
			rotation = FBXVector3ToUnityVector3( model.GetPivotRotation() );
			scale = FBXVector3ToUnityVector3( model.GetPivotScale() );
		}

		static Mesh GetMesh( FBXModelPtr model, float scaleFactor, Vector3 rotationOffset )
		{
			if( model.GetVertexCount() <= 0 )
			{
				return null;
			}

			// vertice
			List<Vector3> vertice = new List<Vector3>();
			for( int i = 0; i < model.GetVertexCount(); ++i )
			{
				vertice.Add( ( FBXVector3ToUnityVector3( model.GetVertex( i ) ) - FBXVector3ToUnityVector3( model.GetPivotScale() ) ) * scaleFactor );
			}

			// color
			List<Color> colors = new List<Color>();
			for( int i = 0; i < model.GetColorCount(); ++i )
			{
				colors.Add( FBXColorToUnityColor( model.GetColor( i ) ) );
			}

			// uv
			List<List<Vector2>> uvs = new List<List<Vector2>>();
			for( int iUVLayer = 0; iUVLayer < model.GetUVLayerCount(); ++iUVLayer )
			{
				List <Vector2> uv = new List<Vector2>();
				for( int iUV = 0; iUV < model.GetUVCount( iUVLayer ); ++iUV )
				{
					uv.Add( FBXVector2ToUnityVector2( model.GetUV( iUVLayer, iUV ) ) );
				}
				uvs.Add( uv );
			}

			// normal
			List<Vector3> normals = new List<Vector3>();
			for( int i = 0; i < model.GetNormalCount(); ++i )
			{
				normals.Add( FBXVector3ToUnityVector3( model.GetNormal( i ) ) );
			}

			// tangent
			List<Vector4> tangents = new List<Vector4>();
			for( int i = 0; i < model.GetTangentCount(); ++i )
			{
				tangents.Add( FBXVector4ToUnityVector4( model.GetTangent( i ) ) );
			}

			// indice, flip indice
			//Debug.LogFormat( "model has {0} submesh", model.GetMaterialCount() );
			for( int i = 0; i < model.GetMaterialCount(); ++i )
			{
				int materialPolygonCount = model.GetMaterialPolygonCount( i );
				//Debug.LogFormat( "material {0} has {1} polygons", i, materialPolygonCount );
			}

			List<List<int>> indice = new List<List<int>>();
			for( int iMaterial = 0; iMaterial < model.GetMaterialCount(); ++iMaterial )
			{
				List<int> submesh = new List<int>();
				for( int i = 0; i < model.GetIndiceCount( iMaterial ); i += 3 )
				{
					submesh.Add( model.GetIndex( iMaterial, i ) );
					submesh.Add( model.GetIndex( iMaterial, i + 2 ) );
					submesh.Add( model.GetIndex( iMaterial, i + 1 ) );
				}
				indice.Add( submesh );
			}

			Mesh mesh = new Mesh();
			mesh.SetVertices( vertice );
			//Debug.LogFormat( "Mesh {0} has {1} vertice", model.GetName(), vertice.Count );

			if( colors.Count > 0 )
			{
				mesh.SetColors( colors );
				//Debug.LogFormat( "Mesh {0} has {1} colors", model.GetName(), colors.Count );
			}
			//else
			//{
			//	Debug.LogWarningFormat( "Mesh {0} has no colors", model.GetName() );
			//}

			if( uvs.Count > 0 )
			{
				for( int i = 0; i < uvs.Count; ++i )
				{
					mesh.SetUVs( i, uvs[i] );
					//Debug.LogFormat( "Mesh {0} layer {1} has {2} uv", model.GetName(), i, uvs[i].Count );
				}
			}
			//else
			//{
			//	Debug.LogWarningFormat( "Mesh {0} has no uvs", model.GetName() );
			//}

			if( normals.Count > 0 )
			{
				mesh.SetNormals( normals );
				//Debug.LogFormat( "Mesh {0} has {1} normals", model.GetName(), normals.Count );
			}
			//else
			//{
			//	Debug.LogWarningFormat( "Mesh {0} has no normals", model.GetName() );
			//}

			if( tangents.Count > 0 )
			{
				mesh.SetTangents( tangents );
				// Debug.LogFormat( "Mesh {0} has {1} tangents", model.GetName(), tangents.Count );
			}
			//else
			//{
			//	Debug.LogWarningFormat( "Mesh {0} has no tangents", model.GetName() );
			//}

			mesh.subMeshCount = indice.Count;
			for( int i = 0; i < indice.Count; ++i )
			{
				mesh.SetIndices( indice[i].ToArray(), MeshTopology.Triangles, i );
				//Debug.LogFormat( "submesh {0} has {1} indice", i, indice[i].Count );
				//for( int j = 0; j < indice[i].Count; ++j )
				//{
				//	Debug.LogFormat( "submesh indice[{0}] = {1}", i, indice[i][j] );
				//}
			}

			// mesh.RecalculateNormals();
			mesh.Optimize();

			return mesh;
		}

		static string GetTextureFullPath0( string modelPath, string textureFileName )
		{
			return Path.GetFullPath( modelPath ) + Path.DirectorySeparatorChar + textureFileName;
		}

		static string GetTextureFullPath1( string modelPath, string textureFileName )
		{
			return Path.GetFullPath( modelPath ) + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Textures" + Path.DirectorySeparatorChar + textureFileName;
		}

		static Texture2D GetTexture( string path, TextureFormat format, bool mipmap )
		{
			if( File.Exists( path ) )
			{
				WWW www = new WWW( "file:///" + path );
				while( !www.isDone ) ;
				Texture2D text = new Texture2D( www.texture.width, www.texture.height, format, mipmap );
				www.LoadImageIntoTexture( text );
				return text;
			}

			return null;
		}

		static Texture TryLoadTexture( string modelPath, string textureFileName, TextureFormat format, bool mipmap )
		{
			string path = GetTextureFullPath0( modelPath, textureFileName );
			if( File.Exists( path ) )
			{
				return GetTexture( path, format, mipmap );
			}

			path = GetTextureFullPath1( modelPath, textureFileName );
			if( File.Exists( path ) )
			{
				return GetTexture( path, format, mipmap );
			}

			return null;
		}

		static Texture2D ConvertTextureToNormal( Texture2D texture )
		{
			if( texture == null )
			{
				return null;
			}

			Texture2D ret = new Texture2D( texture.width, texture.height, TextureFormat.ARGB32, false );
			Color c = new Color();
			for( int x = 0; x < texture.width; ++x )
			{
				for( int y = 0; y < texture.height; ++y )
				{
					c.r = 0;
					c.g = texture.GetPixel( x, y ).r * 0.5f;
					c.b = 0;
					c.a = texture.GetPixel( x, y ).b;
					ret.SetPixel( x, y, c );
				}
			}

			ret.Apply();
			return ret;
		}

		static void GetMaterial( string modelPath, FBXModelPtr model, ref List<Material> materials )
		{
			if( model.GetMaterialCount() > 0 )
			{
				for( int i = 0; i < model.GetMaterialCount(); ++i )
				{
					FBXMaterialPtr mat = model.GetMaterial( i );
					if( !mat.IsNull() )
					{
						Material material = new Material( Shader.Find( "Standard" ) );
						material.name = mat.GetName();

						if( mat.Exist( "Diffuse" ) )
						{
							material.SetColor( "_Color", FBXVector3ToUnityColor( mat.GetVector3( "Diffuse" ) ) );
						}

						if( mat.Exist( "DiffuseColor" ) )
						{
							material.name = Path.GetFileNameWithoutExtension( mat.GetString( "DiffuseColor" ) );
							material.mainTexture = TryLoadTexture( modelPath, Path.GetFileName( mat.GetString( "DiffuseColor" ) ), TextureFormat.RGBA32, true );
						}

						if( mat.Exist( "NormalMap" ) )
						{
							material.SetTexture( "_BumpMap", TryLoadTexture( modelPath, Path.GetFileName( mat.GetString( "NormalMap" ) ), TextureFormat.RGBA32, true ) );
						}

						materials.Add( material );
					}
				}
			}
		}

		static GameObject CreateGameObject( string modelPath, FBXModelPtr model, float unitScaleFactor, Transform parent, Vector3 parentScalePivot )
		{			
			GameObject obj = new GameObject();
			// GameObject obj = GameObject.Instantiate<GameObject>();
			if( obj == null )
			{
				return null;
			}

			// set parent
			obj.transform.SetParent( parent, false );

			// set name
			obj.name = model.GetName();

			// set transform
			Vector3 pivotP = Vector3.zero;
			Vector3 pivotR = Vector3.zero;
			Vector3 pivotS = Vector3.one;

			Vector3 modelP = Vector3.zero;
			Quaternion modelR = Quaternion.identity;
			Vector3 modelS = Vector3.one;

			GetPivotTransform( model, out pivotP, out pivotR, out pivotS );
			GetTrasform( model, out modelP, out modelR, out modelS );
			obj.transform.localPosition = (
				FBXVector3ToUnityVector3( model.GetTranslation() ) +
				FBXVector3ToUnityVector3( model.GetPivotRotation() ) +
				FBXVector3ToUnityVector3( model.GetRotationOffset() ) -
				parentScalePivot
				) * unitScaleFactor;
			obj.transform.localRotation = FBXVector3ToUnityQuaternion( model.GetPreRotation() ) * modelR * Quaternion.Inverse( FBXVector3ToUnityQuaternion( model.GetPostRotation() ) );
			obj.transform.localScale = modelS;

			// set mesh
			Mesh mesh = GetMesh( model, unitScaleFactor, FBXVector3ToUnityVector3( model.GetRotationOffset() ) );
			if( mesh != null )
			{
				mesh.name = model.GetName();
				MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
				meshFilter.mesh = mesh;
			}

			// set material
			List<Material> materials = new List<Material>();
			GetMaterial( modelPath, model, ref materials );
			if( materials.Count > 0 && mesh != null )
			{
				MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
				if( materials.Count == 1 )
				{
					if( meshRenderer.material != null )
					{
						GameObject.DestroyImmediate( meshRenderer.material );
					}

					meshRenderer.material = materials[0];
				}
				else
				{
					for( int i = 0; i < materials.Count; ++i )
					{
						if( i < meshRenderer.materials.Length )
						{
							if( meshRenderer.materials[i] != null )
							{
								GameObject.DestroyImmediate( meshRenderer.materials[i] );
							}
						}

						meshRenderer.materials = materials.ToArray();
					}
				}
			}

			// create childs
			for( int i = 0; i < model.GetChildCount(); ++i )
			{
				CreateGameObject( modelPath, model.GetChild( i ), unitScaleFactor, obj.transform, FBXVector3ToUnityVector3( model.GetPivotScale() ) );
			}

			return obj;
		}

		static GameObject CreateGameObject( string modelPath, FBXScenePtr scene )
		{
			// get unity scale factor and scale to meter for Unity
			float unitScaleFactor = 1.0f / scene.GetGlobalSettings().GetUnitScaleFactor();

			FBXModelPtr model = scene.GetModel();
			if( model == null )
			{
				return null;
			}
			
			// skip scene root if only one node under root, same as Unity does
			if( model.GetChildCount() == 1 )
			{
				model = model.GetChild( 0 );
			}

			GameObject ret = CreateGameObject( modelPath, model, unitScaleFactor, null, Vector3.zero );
			if( ret != null )
			{
				ret.gameObject.name = Path.GetFileNameWithoutExtension( modelPath );
				ret.gameObject.transform.localPosition /= unitScaleFactor;
				ret.gameObject.transform.localScale /= unitScaleFactor;
			}

			return ret;
		}

		public static GameObject Import( string modelPath )
		{
			Debug.LogFormat( "import {0}", modelPath );
			if( !File.Exists( modelPath ) )
			{
				Debug.LogErrorFormat( "Import {0} failed, file not exist.", modelPath );
				return null;
			}

			if( Path.GetExtension( modelPath ).ToLower() == ".fbx" )
			{
				FBXScenePtr scene = fbxImporter.ImportFile( modelPath );
				if( scene != null && !scene.IsNull() )
				{
					return CreateGameObject( modelPath, scene );
				}
				else
				{
					Debug.LogWarningFormat( "{0} scene is empty", modelPath );
				}
			}
			else
			{
				Debug.LogErrorFormat( "Not support file format {0}", Path.GetExtension( modelPath ) );
			}

			return null;
		}
    }
}

#pragma once

#include "FBXImporterCommon.h"
#include "RCPtr.h"

namespace FBXImporterUnmanaged
{
	class FBXGlobalSettings;
	typedef RCPtr<FBXGlobalSettings> FBXGlobalSettingsPtr;

	class FBXIMPORTER_CORE FBXGlobalSettings : public RCObject
	{
	public:
		enum UpVector : int
		{
			X,
			Y,
			Z
		};

		enum FrontVector : int
		{
			Odd,
			Even
		};

		enum CoordinateSystem : int
		{
			LeftHand,
			RightHand
		};

		float				unitScaleFactor;
		float				originalScaleFactor;
		UpVector			upVector;
		UpVector			originalUpVector;
		FrontVector			frontVector;
		CoordinateSystem	coordinateSystem;

	public:
		static FBXGlobalSettingsPtr Create();
		FBXGlobalSettings();
		virtual ~FBXGlobalSettings();

		inline void SetUnitScaleFactor( float value ) { unitScaleFactor = value; }
		inline void SetOriginalUnitScaleFactor( float value ) { originalScaleFactor = value; }
		inline void SetUpVector( UpVector value ) { upVector = value; }
		inline void SetOriginalUpVector( UpVector value ) { originalUpVector = value; }
		inline void SetFrontVector( FrontVector value ) { frontVector = value; }
		inline void SetCoordinateSystem( CoordinateSystem value ) { coordinateSystem = value; }
		
		inline float GetUnitScaleFactor() const { return unitScaleFactor; }
		inline float GetOriginalUnitScaleFactor() const { return originalScaleFactor; }
		inline UpVector GetUpVector() const { return upVector; }
		inline UpVector GetOriginalUpVector() const { return originalUpVector; }
		inline FrontVector GetFrontVector() const { return frontVector; }
		inline CoordinateSystem GetCoordinateSystem() const { return coordinateSystem; }
	};
}
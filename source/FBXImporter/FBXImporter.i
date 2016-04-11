%module FBXImporterUnmanaged

%include <windows.i>
%include <std_string.i>
%include "FBXImporterCommon.h"

using namespace FBXImporterUnmanaged;

#pragma SWIG nowarn=503

%include "FBXColor.h"
%include "FBXVector2.h"
%include "FBXVector3.h"
%include "FBXVector4.h"
%include "FBXMatrix4x4.h"
%include "RCPtr.h"

%include "FBXMaterial.h"
%template( FBXMaterialPtr ) FBXImporterUnmanaged::RCPtr<FBXMaterial>;
%include "FBXModel.h"
%template( FBXModelPtr ) FBXImporterUnmanaged::RCPtr<FBXModel>;
%include "FBXGlobalSettings.h"
%template( FBXGlobalSettingsPtr ) FBXImporterUnmanaged::RCPtr<FBXGlobalSettings>;
%include "FBXScene.h"
%template( FBXScenePtr ) FBXImporterUnmanaged::RCPtr<FBXScene>;

%include "FBXImporter.h"

%{
	#include "FBXColor.h"
	#include "FBXVector2.h"
	#include "FBXVector3.h"
	#include "FBXVector4.h"
	#include "FBXMatrix4x4.h"

	#include "RCPtr.h"
	#include "FBXScene.h"
	#include "FBXGlobalSettings.h"
	#include "FBXMaterial.h"
	#include "FBXModel.h"
	#include "FBXImporter.h"
%}
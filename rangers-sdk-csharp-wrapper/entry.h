#include <new>
#include <cassert>
#include <cstdint>
#include <cstring>
#include <windows.h>
#include <d3d11.h>

namespace csl::math {
	class Vector2 {
	public:
		float x; float y;
	};

	class alignas(16) Vector3 {
	public:
		float x; float y; float z;

		inline bool operator==(const Vector3& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Vector3& other) const {
			return !operator==(other);
		}
	};

	class alignas(16) Vector4 {
	public:
		float x; float y; float z; float w;
	};

	class alignas(16) Quaternion  {
	public:
		float x; float y; float z; float w;

		inline bool operator==(const Quaternion& other) const {
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		inline bool operator!=(const Quaternion& other) const {
			return !operator==(other);
		}
	};

	class alignas(16) Matrix44 {
	public:
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	class alignas(16) Matrix34 {
	public:
		Vector4 t; Vector4 u; Vector4 v; Vector4 w;
	};

	class Position {
	public:
		float x; float y; float z;

		inline bool operator==(const Position& other) const {
			return x == other.x && y == other.y && z == other.z;
		}

		inline bool operator!=(const Position& other) const {
			return !operator==(other);
		}
	};

	class Rotation {
	public:
		float x; float y; float z; float w;

		inline bool operator==(const Rotation& other) const {
			return x == other.x && y == other.y && z == other.z && w == other.w;
		}

		inline bool operator!=(const Rotation& other) const {
			return !operator==(other);
		}
	};
}

#define NO_EIGEN_MATH
#define NO_METADATA

#include "../rangers-sdk/include/rangers-sdk.h"

// #include <cstdint>

// // class GameServiceClass { int meh; };
// // class Vector3 { float x; float y; float z; };

// // #define GAMESERVICE_CLASS_DECLARATION(ClassName) private:\
// // 		static const GameServiceClass gameServiceClass;\
// // 		ClassName(void* allocator);\
// // 		static Foo* Create(void* allocator);\
// // 	public:\
// // 		static const GameServiceClass* GetClass();

// // class Base {
// // public:
// // 	virtual ~Base() = default;
// // };

// // class IListener {
// // public:
// // 	virtual ~IListener() = default;
// // 	virtual void ListenX() {}
// // };

// // class Foo : public Base {
// // 	int ass;
// // public:
// // 	virtual uint64_t UnkFunc1() = 0;
// // 	virtual uint64_t UnkFunc2() = 0;
// // 	virtual uint64_t UnkFunc3() = 0;
// // 	virtual uint64_t UnkFunc4() = 0;
// // 	virtual uint64_t UnkFunc5() = 0;
// // 	virtual uint64_t UnkFunc6() = 0;
// // 	virtual bool PerformRayCastClosest(const Vector3& from, const Vector3& to, uint32_t filterMask) = 0;
// // 	virtual bool PerformRayCastAllHits(const Vector3& from, const Vector3& to, uint32_t filterMask) = 0;
// // 	virtual uint64_t UnkFunc9() = 0;
// // 	virtual uint64_t UnkFunc10() = 0;
// // 	virtual uint64_t UnkFunc11() = 0;
// // 	virtual uint64_t UnkFunc12() = 0;
// // 	virtual uint64_t UnkFunc13() = 0;
// // 	bool RayCastClosest(const Vector3& from, const Vector3& to, uint32_t filterMask);
// // 	bool RayCastAllHits(const Vector3& from, const Vector3& to, uint32_t filterMask);

// // 	GAMESERVICE_CLASS_DECLARATION(Foo);
// // };
// namespace foo {
// class Dorld {
// 	virtual uint64_t UnkFunc1() = 0;
// };
// }

// class Bar : public foo::Dorld {
// public:
// 	virtual uint64_t UnkFunc1() override;
// };


// // const GameServiceClass* Foo::GetClass() {
// // 	return Bar::GetClass();
// // }

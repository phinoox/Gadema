namespace Gadema.Tests.DependencyTests.Functionality.Models;


// ─── Cycle test types ─────────────────────────────────────────────────────
using Gadema.Core.DependencyTracking;

[ModelDependency(typeof(CycleTypeB))]
public class CycleTypeA { }

[ModelDependency(typeof(CycleTypeA))]
public class CycleTypeB { }

// ─── Topological order test types ─────────────────────────────────────────
// Linear chain: TopoTypeD → TopoTypeC → TopoTypeB → TopoTypeA
[ModelDependency(typeof(TopoTypeB))]
public class TopoTypeA { }

[ModelDependency(typeof(TopoTypeC))]
public class TopoTypeB { }

[ModelDependency(typeof(TopoTypeD))]
public class TopoTypeC { }

[ModelDependency(typeof(RootMarker))]
public class TopoTypeD { } // root — no dependencies
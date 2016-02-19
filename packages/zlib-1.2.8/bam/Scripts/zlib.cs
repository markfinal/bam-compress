using Bam.Core;
namespace zlib
{
    [Bam.Core.ModuleGroup("Thirdparty/Zlib")]
    class ZLib :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/*.h");
            var source = this.CreateCSourceContainer("$(packagedir)/*.c");

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)"));
                    }
                });

            source.PrivatePatch(settings =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    var visualCCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != visualCCompiler)
                    {
                        visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        compiler.PreprocessorDefines.Add("_WINDOWS");
                    }
                });
        }
    }

    namespace tests
    {
        sealed class example :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/test/example.c");
                this.CompileAndLinkAgainst<ZLib>(source);

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }
            }
        }

#if false
        sealed class infocover :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/test/infcover.c");
                this.CompileAndLinkAgainst<ZLib>(source);

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }
            }
        }
#endif

        sealed class minigzip :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/test/minigzip.c");
                this.CompileAndLinkAgainst<ZLib>(source);

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }
            }
        }
    }
}

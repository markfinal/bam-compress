#region License
// Copyright (c) 2010-2017, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
using Bam.Core;
namespace zlib
{
    [Bam.Core.ModuleGroup("Thirdparty/Zlib")]
    [C.Thirdparty("$(packagedir)/win32/zlib1.rc")]
    class ZLib :
        C.DynamicLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["OutputName"] = Bam.Core.TokenizedString.CreateVerbatim("zlib");

            this.Macros["MajorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("1");
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("2");
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim("11");

            this.CreateHeaderContainer("$(packagedir)/*.h");
            var source = this.CreateCSourceContainer("$(packagedir)/*.c");

            source["crc32.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("4127"); // zlib-1.2.11\crc32.c(215): warning C4127: conditional expression is constant
                        }
                    }));

            source["deflate.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("4127"); // zlib-1.2.11\deflate.c(1495): warning C4127: conditional expression is constant
                            compiler.DisableWarnings.AddUnique("4244"); // zlib-1.2.11\deflate.c(1693): warning C4244: '=': conversion from 'unsigned int' to 'Bytef', possible loss of data
                        }
                    }));

            source["gzlib.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS"); // zlib-1.2.11\gzlib.c(193): warning C4996: 'wcstombs': This function or variable may be unsafe
                            compiler.DisableWarnings.AddUnique("4996"); // zlib-1.2.11\gzlib.c(245): warning C4996: 'open': The POSIX name for this item is deprecated
                        }
                    }));

            source["gzread.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS"); // zlib-1.2.11\gzread.c(41): warning C4996: 'strerror': This function or variable may be unsafe
                            compiler.DisableWarnings.AddUnique("4996"); // zlib-1.2.11\gzread.c(35): warning C4996: 'read': The POSIX name for this item is deprecated.
                            compiler.DisableWarnings.AddUnique("4245"); // zlib-1.2.11\gzread.c(317): warning C4245: '=': conversion from 'int' to 'unsigned int', signed/unsigned mismatch
                            compiler.DisableWarnings.AddUnique("4267"); // zlib-1.2.11\gzread.c(319): warning C4267: '=': conversion from 'size_t' to 'unsigned int', possible loss of data
                        }
                    }));

            source["gzwrite.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS"); // zlib-1.2.11\gzwrite.c(91): warning C4996: 'strerror': This function or variable may be unsafe
                            compiler.DisableWarnings.AddUnique("4996"); // zlib-1.2.11\gzwrite.c(89): warning C4996: 'write': The POSIX name for this item is deprecated
                            compiler.DisableWarnings.AddUnique("4267"); // zlib-1.2.11\gzwrite.c(212): warning C4267: '=': conversion from 'size_t' to 'unsigned int', possible loss of data
                        }
                    }));

            source["trees.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("4244"); // zlib-1.2.11\trees.c(724): warning C4244: '+=': conversion from 'int' to 'ush', possible loss of data
                        }
                    }));

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

                    var cCompiler = settings as C.ICOnlyCompilerSettings;
                    cCompiler.LanguageStandard = C.ELanguageStandard.C89;

                    var visualCCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != visualCCompiler)
                    {
                        visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        compiler.PreprocessorDefines.Add("_WINDOWS");
                        compiler.PreprocessorDefines.Add("ZLIB_DLL");
                        compiler.DisableWarnings.AddUnique("4131"); // zlib-1.2.11\adler32.c(64): warning C4131: 'adler32_z': uses old-style declarator
                    }

                    var mingwCompiler = settings as MingwCommon.ICommonCompilerSettings;
                    if (null != mingwCompiler)
                    {
                        mingwCompiler.AllWarnings = true;
                        mingwCompiler.ExtraWarnings = true;
                        mingwCompiler.Pedantic = true;
                    }

                    var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                    if (null != gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;

                        compiler.PreprocessorDefines.Add("HAVE_UNISTD_H"); // for lseek etc

                        // because this is not c99
                        compiler.PreprocessorDefines.Add("NO_snprintf");
                        compiler.PreprocessorDefines.Add("NO_vsnprintf");

                        // flip the normal rules of visibility - make everything visible, and hide internals
                        compiler.PreprocessorDefines.Add("HAVE_HIDDEN");
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }

                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;

                        compiler.PreprocessorDefines.Add("HAVE_UNISTD_H"); // for lseek etc

                        // flip the normal rules of visibility - make everything visible, and hide internals
                        compiler.PreprocessorDefines.Add("HAVE_HIDDEN");
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;

                    #if false
                        if (source.Compiler.IsAtLeast(700))
                        {
                            compiler.DisableWarnings.AddUnique("shift-negative-value"); // zlib-1.2.8/inflate.c:1507:61: error: shifting a negative signed value is undefined [-Werror,-Wshift-negative-value]
                        }
                    #endif
                    }
                });

            if (this.Linker is VisualCCommon.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();

                if (null != this.WindowsVersionResource)
                {
                    this.WindowsVersionResource.UsePublicPatches(C.DefaultToolchain.C_Compiler(this.BitDepth)); // for limits.h
                }
            }

            this.WindowsVersionResource.PrivatePatch(settings =>
                {
                    var mingwRC = settings as MingwCommon.ICommonWinResourceCompilerSettings;
                    if (null != mingwRC)
                    {
                        var rc = settings as C.ICommonWinResourceCompilerSettings;
                        rc.PreprocessorDefines.Add("GCC_WINDRES");
                    }
                });
        }
    }

    namespace tests
    {
        [Bam.Core.ModuleGroup("Thirdparty/Zlib/tests")]
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

                source.PrivatePatch(settings =>
                    {
                        var cCompiler = settings as C.ICOnlyCompilerSettings;
                        cCompiler.LanguageStandard = C.ELanguageStandard.C89;

                        var visualCCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != visualCCompiler)
                        {
                            visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;

                        #if false
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                        #endif
                        }

                        var mingwCompiler = settings as MingwCommon.ICommonCompilerSettings;
                        if (null != mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = true;
                            mingwCompiler.ExtraWarnings = true;
                            mingwCompiler.Pedantic = true;
                        }

                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            gccCompiler.AllWarnings = true;
                            gccCompiler.ExtraWarnings = true;
                            gccCompiler.Pedantic = true;
                        }

                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            clangCompiler.AllWarnings = true;
                            clangCompiler.ExtraWarnings = true;
                            clangCompiler.Pedantic = true;
                        }
                    });

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }

                this.PrivatePatch(settings =>
                    {
                        var gccLinker = settings as GccCommon.ICommonLinkerSettings;
                        if (null != gccLinker)
                        {
                            gccLinker.CanUseOrigin = true;
                            gccLinker.RPath.AddUnique("$ORIGIN");
                        }
                    });
            }
        }

#if false
        [Bam.Core.ModuleGroup("Thirdparty/Zlib/tests")]
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

                this.PrivatePatch(settings =>
                    {
                        var gccLinker = settings as GccCommon.ICommonLinkerSettings;
                        if (null != gccLinker)
                        {
                            gccLinker.CanUseOrigin = true;
                            gccLinker.RPath.AddUnique("$ORIGIN");
                        }
                    });
            }
        }
#endif

        [Bam.Core.ModuleGroup("Thirdparty/Zlib/tests")]
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

                source.PrivatePatch(settings =>
                    {
                        var cCompiler = settings as C.ICOnlyCompilerSettings;
                        cCompiler.LanguageStandard = C.ELanguageStandard.C89;

                        var visualCCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != visualCCompiler)
                        {
                            visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        }

                        var mingwCompiler = settings as MingwCommon.ICommonCompilerSettings;
                        if (null != mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = true;
                            mingwCompiler.ExtraWarnings = true;
                            mingwCompiler.Pedantic = true;
                        }

                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            gccCompiler.AllWarnings = false;
                            gccCompiler.ExtraWarnings = true;
                            gccCompiler.Pedantic = true;
                        }

                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            clangCompiler.AllWarnings = true;
                            clangCompiler.ExtraWarnings = true;
                            clangCompiler.Pedantic = true;
                        }
                    });

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }

                this.PrivatePatch(settings =>
                    {
                        var gccLinker = settings as GccCommon.ICommonLinkerSettings;
                        if (null != gccLinker)
                        {
                            gccLinker.CanUseOrigin = true;
                            gccLinker.RPath.AddUnique("$ORIGIN");
                        }
                    });
            }
        }

        sealed class TestRuntime :
            Publisher.Collation
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var app = this.Include<example>(C.ConsoleApplication.Key, EPublishingType.ConsoleApplication);
                this.Include<minigzip>(C.ConsoleApplication.Key, ".", app);
                this.Include<ZLib>(C.DynamicLibrary.Key, ".", app);
            }
        }
    }
}

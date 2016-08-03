#region License
// Copyright (c) 2010-2016, Mark Final
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
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim("8");

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
                        compiler.PreprocessorDefines.Add("ZLIB_DLL");
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

                        compiler.PreprocessorDefines.Add("HAVE_HIDDEN");
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }

                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        clangCompiler.AllWarnings = false;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;

                        compiler.PreprocessorDefines.Add("HAVE_HIDDEN");
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                    }
                });

            if (this.Linker is VisualCCommon.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
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

                source.PrivatePatch(settings =>
                    {
                        var visualCCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != visualCCompiler)
                        {
                            visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level3;

                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
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

                source.PrivatePatch(settings =>
                    {
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
            }
        }
    }
}

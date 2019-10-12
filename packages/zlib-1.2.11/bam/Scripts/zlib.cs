#region License
// Copyright (c) 2010-2019, Mark Final
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
        Init()
        {
            base.Init();

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                var versionScript = Bam.Core.Graph.Instance.FindReferencedModule<VersionScript>();
                this.DependsOn(versionScript);
                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
                        {
                            gccLinker.VersionScript = versionScript.InputPath;
                        }
                    });
            }

            this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("zlib");

            this.SetSemanticVersion(1, 2, 11);

            this.CreateHeaderCollection("$(packagedir)/*.h");
            var source = this.CreateCSourceCollection("$(packagedir)/*.c");

            if (source.Compiler is VisualCCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new VisualC.WarningSuppression.Zlib());
            }
            else if (source.Compiler is GccCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Gcc.WarningSuppression.Zlib());
            }
            else if (source.Compiler is ClangCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Clang.WarningSuppression.Zlib());
            }

            source.PrivatePatch(settings =>
                {
                    var preprocessor = settings as C.ICommonPreprocessorSettings;

                    var cCompiler = settings as C.ICOnlyCompilerSettings;
                    cCompiler.LanguageStandard = C.ELanguageStandard.C89;

                    if (settings is VisualCCommon.ICommonCompilerSettings visualCCompiler)
                    {
                        visualCCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        preprocessor.PreprocessorDefines.Add("_WINDOWS");
                        preprocessor.PreprocessorDefines.Add("ZLIB_DLL");

                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("4131"); // happens to a lot of files
                    }

                    if (settings is MingwCommon.ICommonCompilerSettings mingwCompiler)
                    {
                        mingwCompiler.AllWarnings = true;
                        mingwCompiler.ExtraWarnings = true;
                        mingwCompiler.Pedantic = true;
                    }

                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;

                        preprocessor.PreprocessorDefines.Add("HAVE_UNISTD_H"); // for lseek etc

                        // because this is not c99
                        preprocessor.PreprocessorDefines.Add("NO_snprintf");
                        preprocessor.PreprocessorDefines.Add("NO_vsnprintf");

                        // flip the normal rules of visibility - make everything visible, and hide internals
                        preprocessor.PreprocessorDefines.Add("HAVE_HIDDEN");
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }

                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;

                        preprocessor.PreprocessorDefines.Add("HAVE_UNISTD_H"); // for lseek etc

                        // flip the normal rules of visibility - make everything visible, and hide internals
                        preprocessor.PreprocessorDefines.Add("HAVE_HIDDEN");
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                    }
                });

            if (null != this.WindowsVersionResource)
            {
                this.WindowsVersionResource.PrivatePatch(settings =>
                    {
                        if (settings is MingwCommon.ICommonWinResourceCompilerSettings)
                        {
                            var rc = settings as C.ICommonWinResourceCompilerSettings;
                            rc.PreprocessorDefines.Add("GCC_WINDRES");
                        }
                    });
            }
        }
    }
}

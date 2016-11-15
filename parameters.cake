public class BuildParameters
{
    public String Solution = "./src/NSaga.sln";

    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPullRequest  { get; private set; }
    public bool SkipGitVersion { get; private set; }
    public ReleaseNotes ReleaseNotes { get; private set; }
    public bool IsMasterBranch { get; private set; }

    public string Version { get; private set; }
    public string SemVersion { get; private set; }



    public void Initialize(ICakeContext context)
    {
        context.Information("Executing GitVersion");
        var result = context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfoFilePath = "**/properties/AssemblyInfo.cs",
            UpdateAssemblyInfo = true,
        });
        Version = result.MajorMinorPatch ?? "0.1.0";
        SemVersion = result.LegacySemVerPadded ?? "0.1.0";

		// print gitversion
        context.GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });
    }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var configuration = context.Argument("configuration", "Release");
        var buildSystem = context.BuildSystem();
		var isMaster = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch);

		context.Information("IsTagged: {0}", IsBuildTagged(buildSystem));
		context.Information("IsMasterBranch: {0}", isMaster);

        return new BuildParameters {
            Target = target,
            Configuration = configuration,
            
            // BuildDir = ProjectDir + "bin/" + configuration,

            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAppVeyor = buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
            IsTagged = IsBuildTagged(buildSystem),


            IsMasterBranch = isMaster,
            ReleaseNotes = context.ParseReleaseNotes("./ReleaseNotes.md"),
            SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("CAKE_SKIP_GITVERSION")),
        };
    }

    private static bool IsBuildTagged(BuildSystem buildSystem)
    {
        return buildSystem.AppVeyor.Environment.Repository.Tag.IsTag
            && !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name);
    }

    public string BuildResultDir
    {
        get 
        {
            return "./build-results/v" + SemVersion + "/";
        }
    }

    public string ResultBinDir
    {
        get 
        {
            return BuildResultDir + "bin";
        }
    }


    public string ResultNugetPath
    {
        get 
        {
            return BuildResultDir + "NSaga." + Version + ".nupkg";
        }
    }

    public bool ShouldPublishToNugetOrg
    {
        get
        {
            return !IsLocalBuild && !IsPullRequest && IsTagged && IsMasterBranch;
        }
    }

    public bool ShouldPublishToMyGet
    {
        get
        {
            return !IsLocalBuild && !IsPullRequest && !IsTagged || IsMasterBranch;
        }
    }    
}


﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using CliWrap.Builders;

namespace CliWrap;

/// <summary>
/// Instructions for running a process.
/// </summary>
public partial class Command(
    string targetFilePath,
    string arguments,
    string workingDirPath,
    Credentials credentials,
    IReadOnlyDictionary<string, string?> environmentVariables,
    CommandResultValidation validation,
    PipeSource standardInputPipe,
    PipeTarget standardOutputPipe,
    PipeTarget standardErrorPipe,
    TimeSpan? pipingTimeout
) : ICommandConfiguration
{
    /// <summary>
    /// Initializes an instance of <see cref="Command" />.
    /// </summary>
    public Command(string targetFilePath)
        : this(
            targetFilePath,
            string.Empty,
            Directory.GetCurrentDirectory(),
            Credentials.Default,
            new Dictionary<string, string?>(),
            CommandResultValidation.ZeroExitCode,
            PipeSource.Null,
            PipeTarget.Null,
            PipeTarget.Null,
            null
        ) { }

    /// <inheritdoc />
    public string TargetFilePath { get; } = targetFilePath;

    /// <inheritdoc />
    public string Arguments { get; } = arguments;

    /// <inheritdoc />
    public string WorkingDirPath { get; } = workingDirPath;

    /// <inheritdoc />
    public Credentials Credentials { get; } = credentials;

    /// <inheritdoc />
    public TimeSpan? PipingTimeout { get; } = pipingTimeout;

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string?> EnvironmentVariables { get; } =
        environmentVariables;

    /// <inheritdoc />
    public CommandResultValidation Validation { get; } = validation;

    /// <inheritdoc />
    public PipeSource StandardInputPipe { get; } = standardInputPipe;

    /// <inheritdoc />
    public PipeTarget StandardOutputPipe { get; } = standardOutputPipe;

    /// <inheritdoc />
    public PipeTarget StandardErrorPipe { get; } = standardErrorPipe;

    /// <summary>
    /// Creates a copy of this command, setting the target file path to the specified value.
    /// </summary>
    [Pure]
    public Command WithTargetFile(string targetFilePath) =>
        new(
            targetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the arguments to the specified value.
    /// </summary>
    /// <remarks>
    /// Avoid using this overload, as it requires the arguments to be escaped manually.
    /// Formatting errors may lead to unexpected bugs and security vulnerabilities.
    /// </remarks>
    [Pure]
    public Command WithArguments(string arguments) =>
        new(
            TargetFilePath,
            arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the arguments to the value
    /// obtained by formatting the specified enumeration.
    /// </summary>
    [Pure]
    public Command WithArguments(IEnumerable<string> arguments, bool escape) =>
        WithArguments(args => args.Add(arguments, escape));

    /// <summary>
    /// Creates a copy of this command, setting the arguments to the value
    /// obtained by formatting the specified enumeration.
    /// </summary>
    // TODO: (breaking change) remove in favor of optional parameter
    [Pure]
    public Command WithArguments(IEnumerable<string> arguments) => WithArguments(arguments, true);

    /// <summary>
    /// Creates a copy of this command, setting the arguments to the value
    /// configured by the specified delegate.
    /// </summary>
    [Pure]
    public Command WithArguments(Action<ArgumentsBuilder> configure)
    {
        var builder = new ArgumentsBuilder();
        configure(builder);

        return WithArguments(builder.Build());
    }

    /// <summary>
    /// Creates a copy of this command, setting the working directory path to the specified value.
    /// </summary>
    [Pure]
    public Command WithWorkingDirectory(string workingDirPath) =>
        new(
            TargetFilePath,
            Arguments,
            workingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the piping timeout to the specified value.
    /// </summary>
    public Command WithPipingTimeout(TimeSpan pipingTimeout) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            pipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the user credentials to the specified value.
    /// </summary>
    [Pure]
    public Command WithCredentials(Credentials credentials) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the user credentials to the value
    /// configured by the specified delegate.
    /// </summary>
    [Pure]
    public Command WithCredentials(Action<CredentialsBuilder> configure)
    {
        var builder = new CredentialsBuilder();
        configure(builder);

        return WithCredentials(builder.Build());
    }

    /// <summary>
    /// Creates a copy of this command, setting the environment variables to the specified value.
    /// </summary>
    [Pure]
    public Command WithEnvironmentVariables(
        IReadOnlyDictionary<string, string?> environmentVariables
    ) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            environmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the environment variables to the value
    /// configured by the specified delegate.
    /// </summary>
    [Pure]
    public Command WithEnvironmentVariables(Action<EnvironmentVariablesBuilder> configure)
    {
        var builder = new EnvironmentVariablesBuilder();
        configure(builder);

        return WithEnvironmentVariables(builder.Build());
    }

    /// <summary>
    /// Creates a copy of this command, setting the validation options to the specified value.
    /// </summary>
    [Pure]
    public Command WithValidation(CommandResultValidation validation) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            validation,
            StandardInputPipe,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the standard input pipe to the specified source.
    /// </summary>
    [Pure]
    public Command WithStandardInputPipe(PipeSource source) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            source,
            StandardOutputPipe,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the standard output pipe to the specified target.
    /// </summary>
    [Pure]
    public Command WithStandardOutputPipe(PipeTarget target) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            target,
            StandardErrorPipe,
            PipingTimeout
        );

    /// <summary>
    /// Creates a copy of this command, setting the standard error pipe to the specified target.
    /// </summary>
    [Pure]
    public Command WithStandardErrorPipe(PipeTarget target) =>
        new(
            TargetFilePath,
            Arguments,
            WorkingDirPath,
            Credentials,
            EnvironmentVariables,
            Validation,
            StandardInputPipe,
            StandardOutputPipe,
            target,
            PipingTimeout
        );

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public override string ToString() => $"{TargetFilePath} {Arguments}";
}

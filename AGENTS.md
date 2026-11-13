# Project contribution instructions

## Reuse before implementation

This project is an Android software technician assistant. Prefer working, maintained, and tested software that already exists in the project's GitHub research and source repositories. Do not recreate a library, parser, device adapter, diagnostic workflow, or user-facing feature that is already available and suitable.

Before writing code for a capability:

1. Search this repository's research, source revalidation, attribution, and handoff documents for an existing candidate.
2. Verify the candidate's actual source, current revision, license, maintenance status, platform support, and relevant tests. A README claim or green CI alone is not proof that it fits.
3. Reuse or transplant the smallest suitable component, preserving attribution and license obligations, and record the source revision and any changes.
4. Implement only the specific gap that remains after reuse. Explain why existing candidates do not cover it and add the smallest adapter or glue needed to integrate them.

Do not add a parallel implementation or a throwaway proof of concept when the repository already contains a tested implementation for the same need. If evidence is incomplete, investigate and document the gap before replacing or duplicating code. Keep product changes aligned with the shared contracts and safety rules: explicit device targeting, read-only by default, and explicit approval for operations that change device or Windows state.

# Prefab Lighting Tool

Utility for saving multiple, distinct, light mapping information for individual prefabs for instantiation within alternate scenes.

For an in-depth overview please see the [documentation](./docs/USAGE.md) otherwise the [Quick Start](#quick-start "Quick Start") guide should provide the essentials for the tool's use.

The current version requires Unity 2021.3+.

## Getting Started
* [Quick Start](#quick-start "Quick Start") - Brief introduction to the package
* [Support Questions](#how-to-open-a-support-question "Support Questions") - Check to see if any one else has had the same question.
* [Bug Report](#how-to-submit-a-bug-report "Support Questions") - Check to see if any one else has had the same problem.
* [Feature Requests](#how-to-open-a-feature-request "Support Questions") - Check to see if any one else has had the same idea.


## Quick Start

### Prefab Lightmap Tool

Place your prefabs in the scene with the PrefabLightmap component at their root. Set up your lighting in the editor and use the tool's interface (Window->Rendering->Lighting->Prefab Lightmap Tool) to bake your scene. After the bake is complete you can now spawn your prefabs in different scenes and they will use the light-mapping from the original scene.

*Remember that if you are not instantiating your prefabs at runtime you should remove the static flag from the GameObjects, otherwise static batching will mess with uvs and the lightmap won't work properly.*

### Runtime Light Probes

Preexisting light probes will be updated in real time based on the lighting data found in the scene.

Attach the LightProbeRuntime component to a GameObject within the target scene where prefabs will be instantiated.  In that same scene create (a) uniform light probe group(s) and bake that scene so that light probes are stored in the scene's lighting data.  At runtime the script will wait one frame and then starts calculating the light volume contributions from the lights in the scene to be added to those previously stored light probes.  Best practice is to include the lights with the prefabs that you previously baked for.

## How to Open a Support Question
1. Determine if your question has already been [asked by others](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Aquestion+is%3Aall)
    * If you find the question then there's no need to open another ticket.
    * If you do not feel the question has been thoroughly answered, then comment on it and ask for more clarification
2. Create a [Question Issue](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Custom.md)
    * Add the "question" label to your issue
    * Try to be as detailed as possible
    * Try to use clear and direct language as much as possible
3. Check back, or enable the notification options, to see the answer to your question
    * Keep in mind that everyone is here in their free time and there may be gaps when responses are slow.    Feel free to set the notification settings for the question so you'll know immediately when its answered.

## How to Open a Bug Report
1. Determine if the bug has already been [reported](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Abug+is%3Aopen)
    * If you find that the Issue has already been opened then a new Issue is not needed.
    * Feel free to add your experiences to bug reports and to reopen bug Issues if it crops up again.
2. Create a "Bug Report" on the [Issues page](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Bug_report.md).
    * Add the "bug" label to your issue
    * Make sure that you are very detailed with what you are experiencing
3. Check back often, or enable the notification options, in case others have questions or need further clarification

## How to Open a Feature Request
1. Determine if the same or similar [Feature Requests](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Aenhancement+is%3Aopen+-label%3Adocumentation) already exists.
    * If one does exist, feel free to expand upon it by commenting on the Issue but **do not open a new one**.
2. Create a [Feature Request](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Feature_request.md) on the Issues page.
    * If your Feature Request requires additional/changed documentation then a [Documentation Request](CONTRIBUTING.md#how-to-open-a-documentation-request) is also required
        * You must provide a reference to the Documentation Request in the Feature Request Issue
    * Make sure that you are very detailed with what you think the feature should accomplish, how it should be implemented and it's predicted effect on other features.
    * Add the "enhancement" label to your issue
3. Check back often, or enable the notification options, in the event others have questions about your proposal
4. You will know when the feature is accepted when the "approved" label is assigned to it
    * If there are multiple requests as a part of the feature, the feature will not be moved to the master branch until all dependent parts have been completed.
    * It is important to keep in mind that all new features, even simple changes, often have wide ranging effect which need to be fully explored.
    * Also remember that not all features can be added into the game (but I'm sure we'll try!) and features need to be released in an orderly way so even if the feature is accepted it may not go in immediately.

## Contributing
Please read [CONTRIBUTING](CONTRIBUTING.md) for details on submitting work to the project and our [Code of Conduct](.github/CODE_OF_CONDUCT.md).

## Authors
* **Joachim_Ante** - *Programming* - [Joachim_Ante](https://blog.unity.com/author/cap-joe)

See also the list of [contributors](https://github.com/Enterlucent/PrefabLightmapping/graphs/contributors) who participated in this project.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

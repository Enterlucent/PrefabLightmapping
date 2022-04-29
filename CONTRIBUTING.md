# Quick Start<a id="quick-start"></a>
* [How to Open a Bug Report](#how-to-open-a-bug-report)
* [How to Open a Feature Request](#how-to-open-a-feature-request)
* [How to Open a Documentation Request](#how-to-open-a-documentation-request)
* [How to Submit Code](#how-to-submit-code)

## How to Open a Bug Report<a id="how-to-submit-a-bug-report"></a>
1. Determine if the bug has already been [reported](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Abug+is%3Aopen+is%3Aall)
    * If you find that the Issue has already been opened then a new Issue is not needed.
    * Feel free to add your experiences to bug reports and to reopen bug Issues if it crops up again.
2. Create a "Bug Report" on the [Issues page](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Bug_report.md).
    * Add [BUG] to the title of your issue _(ie [BUG] Cats Get Too Many Cheeseburgers)_
    * Make sure that you are very detailed with what you are experiencing
3. Check back often, or enable the notification options, in case others have questions or need further clarification

## When is it a Feature Request
It might be a little nuanced the difference between a bug vs a change in the expected operation of the game.  In the case of the latter we would want to open a feature request.  The following is a few examples of what would constitute a Feature Request:
* The element is completely missing from the project
* Current behavior is missing a critical element to make it complete
* A current element behaves in a way you feel is detrimental to the project as a whole but is the current, expected, behavior

When you have the next best feature for the PrefabLightmapping project you will first need to let us know about it!

## How to Open a Feature Request<a id="how-to-open-a-feature-request"></a>
1. Determine if the same or similar [Feature Requests](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Aenhancement+is%3Aopen+-label%3Adocumentation) already exists.
    * If one does exist, feel free to expand upon it by commenting on the Issue but **do not open a new one**.
2. Create a [Feature Request](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Feature_request.md) on the Issues page.
    * If your Feature Request requires additional/changed documentation then a [Documentation Request](#how-to-open-a-documentation-request) is also required
        * You must provide a reference to the Documentation Request in the Feature Request Issue
    * Make sure that you are very detailed with what you think the feature should accomplish, how it should be implemented and it's predicted effect on other features.
3. Check back often, or enable the notification options, in the event others have questions about your proposal
4. You will know when the feature is accepted when the "approved" label is assigned to it
    * If there are multiple requests as a part of the feature, the feature will not be moved to the master branch until all dependent parts have been completed.
    * It is important to keep in mind that all new features, even simple changes, often have wide ranging effect which need to be fully explored.
    * Also remember that not all features can be added into the game (but I'm sure we'll try!) and features need to be released in an orderly way so even if the feature is accepted it may not go in immediately.

## How to Open a Documentation Request<a id="how-to-open-a-documentation-request"></a>
1. Determine if the [Documentation Request](https://github.com/Enterlucent/PrefabLightmapping/issues?q=label%3Aenhancement,documentation+is%3Aopen) you want to create already exists.
    * Ask questions about implementation on the Issue thread and/or join the [Discord](https://discord.com/invite/mWDHDqkzeR) server to talk with people in real time.
    * If you find an Issue that is very close to what you'd like to do but missing some critical element, comment on that Issue so the change can be discussed.
2. Open an [Issue](https://github.com/Enterlucent/PrefabLightmapping/issues/new?template=Feature_request.md) explaining your proposed documentation changes/additions
    * Add [DOC] the title of your issue _(ie. [DOC] How to Install PrefabLightmapping)_
3. Others may ask questions or request changes to your proposal so make sure to check back, or enable the notification options!
4. You will know if the Issue is accepted when it's given the "approved" label
    * If there are multiple requests as a part of the request, the request will not be moved to the master branch until all dependent parts have been completed.

## How to Submit Documentation<a id="how-to-submit-documentation"></a>
1. [Find an Issue](#how-to-find-documentation-tasks) to resolve
2. [Fork](https://help.github.com/articles/fork-a-repo/) the [repository](https://github.com/Enterlucent/PrefabLightmapping/)
3. [Create a new branch](https://help.github.com/articles/creating-and-deleting-branches-within-your-repository/) with the format 'issue#{issue-id}-{your-github-user}' _(ie issue#224-Enterlucent)_
4. Make any changes or additions to your new branch
    * Make sure to [keep your forks in sync](https://help.github.com/articles/syncing-a-fork/) with the original repository
    * If you find that you need help with implementation, ask to have the "Help Wanted" label added to the issue as well as seeking others on the [Discord](https://discord.com/invite/mWDHDqkzeR) server
5. When finished, submit a [Pull Request](https://help.github.com/articles/about-pull-requests/) that includes just the changes in the Issue.
    * Others may ask questions or request changes so make sure to check back, or enable the notification options!
6. The [Pull Request](https://help.github.com/articles/about-pull-requests/) will be reviewed and, if approved, will be merged.
    * If there is an Asset or Project Issue in conjunction with the Documentation Issue, the feature will not be moved to the master branch until all dependent parts have been completed.
    * When merged you will be an official contributor.

## How to Submit Code<a id="how-to-submit-code"></a>
1. Setup your [Development Environment](/docs//Dev.Env.Win10.md)
2. [Find an issue](#how-to-find-programing-tasks) to resolve
3. [Fork](https://help.github.com/articles/fork-a-repo/) the [repository](https://github.com/Enterlucent/PrefabLightmapping/)
4. [Create a new branch](https://help.github.com/articles/creating-and-deleting-branches-within-your-repository/) with the format 'issue#{issue-id}-{your-github-user}' _(ie issue#224-mdwigley)_
5. [Update](https://gist.github.com/gitaarik/8735255#keeping-your-submodules-up-to-date) sub-modules
6. Make any changes or additions to your new branch
    * Make sure to [keep your forks in sync](https://help.github.com/articles/syncing-a-fork/) with the original repository
    * If you find that you need help with implementation, ask to have the "Help Wanted" label added to the issue as well as seeking others on the [Discord](https://discord.com/invite/mWDHDqkzeR) server
    * Ensure all files have been white-space formatted
        * Often the default formatting for Visual Studio or MonoDevelop will do
        * If you use Visual Studio you can use the [official format settings](/docs/assets/files/CONTRIBUTING/.editorconfig)
    * Comment your code with both code level and construct definitions
7. When finished, submit a [Pull Request](https://help.github.com/articles/about-pull-requests/) that includes just the changes in the Issue.
    * Others may ask questions or request changes so make sure to check back, or enable the notification options!
8. The [Pull Request](https://help.github.com/articles/about-pull-requests/) will be reviewed and, if approved, will be merged.
    * If there is an Asset or Documentation Issue in conjunction with the Project Issue, the feature will not be moved to the master branch until all dependent parts have been completed.
    * When merged you will be an official contributor!

# Attribution
This CONTRIBUTING document was compiled with the help of [nayafia/contributing-template](https://github.com/nayafia/contributing-template/blob/master/CONTRIBUTING-template.md).

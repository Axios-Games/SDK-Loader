# Unity Package Template

Rapidly set up a [Custom Unity Package](https://docs.unity3d.com/Manual/CustomPackages.html) with this template!

![](https://github.com/Phantasmic-Games/Unity-Package-Template/blob/main/.github/workflows/Using%20Template.gif)

## How to Use
### 'Use this Template' Method
This is the most streamlined and the quickest method. Clicking `Use this template` will trigger the Initialize workflow after you package's repo is generated from the template. Once done, there will be a new commit with the package configured as follows: The package's Organization will be the repo owner's name, the Package name will be the repo's name and the README.md will be cleared.

#### Note:
* If the workflow is not triggered go to `Actions - Initialize - Run workflow` and click `Run workflow`.
* If the workflow fails due to access being revoked, go into the repo owner's `Settings - Actions - General - Workflow permissions` and set `Read and write permissions`.

### Manual Set Up Method
Go to `Code - Download ZIP` and extract it in your Unity project's Packages directory. You could manually edit the `package.json` file in the Inspector and rename the Assembly Definition files to match your package name, but this can be automated with the included bash script. Open a command line program that supports bash and at the Package's root directory, run:
```
bash Init.sh
```
Enter the Organization and Package names and the script will initialize the package and delete unnecessary files (.github folder) just like the GitHub workflow.

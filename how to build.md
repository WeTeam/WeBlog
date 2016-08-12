# How to build WeBlog #

WeBlog builds against and deploys to a local Sitecore instance on your machine. The Sitecore instance can be located anywhere on the local machine including in the same folder as the WeBlog source files. As WeBlog includes build configurations for several different Sitecore versions it is not possible to setup Sitecore in the same folder as the WeBlog project if you want to build against more than one Sitecore version.

If you only need to build against a single Sitecore version and like to work in the web root, then you just need to clone the WeBlog repository into your web root folder. No additional setup is required.

## Building the Project ##

Perform the following before opening the solution in Visual Studio.

1. Ensure you have the desired Sitecore version already installed and working on the local machine.
1. Copy the `deploy.targets.sample` file to `deploy.targets`.
	1. The `deploy.targets` file is a local only file. It's already been added to the `.gitignore` file.
1. Edit the `deploy.targets` file to update the paths to the Sitecore instances.
1. For each of the Sitecore versions you want to build against, update the `SitecorePath` property to reference the web root of the Sitecore instance.
	1. Each `SitecorePath` property includes a `Condition` defining which Sitecore version the property is used for.
1. Open Visual Studio and select the appropriate build configuration.
	1. The numbers of a build configuration refers to the Sitecore version the project will be built for.
1. Rebuild the solution
1. The project will automatically deploy the WeBlog files to the Sitecore instance you build against.

## Restore Sitecore Items ##

In addition to building the project and deploying the files, you must also restore the WeBlog Sitecore items.

1. Copy the `data\serialization` folder to the `data` folder of the Sitecore instance.
1. If using a Sitecore version with RDB (Sitecore 7.5+), copy the folders from inside the `master-rdb-additional` folder into the `master` folder.
1. Access the Serialization utility page `/sitecore/admin/serialization.aspx`.
1. Select the `core` and `master` databases
1. Click the `Update {core, master} databasees` button.
1. Log into the Sitecore desktop.
1. Perform a full publish.

## Building the Themes ##

The WeBlog themes use Grunt as a toolchain and are not included in the Visual Studio projects.

1. Ensure you have Node, NPM, Grunt and Bower installed. Grunt and Bower should be installed globally.

		npm install -g grunt
		npm install -g bower

1. Execute `npm install` in the `Themes` directory to restore the node packages.
1. Execute `bower install` in the `Themes` directory to restore the bower packages.
1. Execute `grunt` in the `Themes` directory to build the themes and deploy to the default (Sitecore 8.0) Sitecore instance
	1. Paths for Sitecore instaces are read from the `deploy.targets` file.
	1. To deploy the themes to a different target instance, pass the `--scversion` parameter to `grunt`:
	
			grunt --scversion=sc7.0

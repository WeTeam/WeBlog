# How to build WeBlog #

WeBlog builds using the public Sitecore NuGet feeds, and optionally deploys to a local Sitecore instance on your machine. The Sitecore instance can be located anywhere on the local machine including in the same folder as the WeBlog source files. As WeBlog includes build configurations for several different Sitecore versions it is not possible to setup Sitecore in the same folder as the WeBlog project if you want to build against more than one Sitecore version.

If you only need to build against a single Sitecore version and like to work in the web root, then you just need to clone the WeBlog repository into your web root folder. No additional setup is required.

## Building the Project ##

Perform the following before opening the solution in Visual Studio.

1. Ensure you have the desired Sitecore version already installed and working on the local machine.
1. Copy the `deploy.props.sample` file to `deploy.props`.
	1. The `deploy.props` file is a local only file. It's already been added to the `.gitignore` file.
1. Edit the `deploy.props` file to update the paths to the Sitecore instances.
1. For each of the Sitecore versions you want to build against, update the `SitecorePath` property to reference the web root of the Sitecore instance.
	1. Each `SitecorePath` property includes a `Condition` defining which Sitecore version the property is used for.
1. Open Visual Studio and select the appropriate build configuration.
	1. The numbers of a build configuration refers to the Sitecore version the project will be built for.
1. Rebuild the solution.
1. The project will automatically deploy the WeBlog files to the Sitecore instance you build against.

## Restore Sitecore Items ##

In addition to building the project and deploying the files, you must also restore the WeBlog Sitecore items.

1. Copy the `data\serialization` folder to the `App_Data` folder of the Sitecore instance.
1. Access the Serialization utility page of the Sitecore instance: `/sitecore/admin/serialization.aspx`.
1. Select the `core` and `master` databases.
1. Click the `Update {core, master} databasees` button.
1. Log into the Sitecore desktop.
1. Perform a full publish.

## Building the Themes ##

The WeBlog themes use Grunt as a toolchain and are not included in the Visual Studio projects.

1. Ensure you have Node, NPM, Grunt and Bower installed. Grunt and Bower should be installed globally.

		npm install -g grunt
		npm install -g bower

1. Execute `npm install` in the `src/Themes` directory to restore the node packages.
1. Execute `bower install` in the `src/Themes` directory to restore the bower packages.
1. Execute `grunt` in the `src/Themes` directory to build the themes and deploy to the default (Sitecore 10.1) Sitecore instance.
	1. Paths for Sitecore instaces are read from the `deploy.props` file above.
	1. To deploy the themes to a different target instance, pass the `--scversion` parameter to `grunt`:
	
			grunt --scversion=sc9.3

## Create Solr Cores ##

Cores must be created in Solr for the WeBlog content search indexes.

1. Open a command prompt.
1. Navigate to the Solr `bin` folder.
1. Create the cores using the `solr create` command:

	solr create -c weblog-master
	solr create -c weblog-web

1. Copy the `managed-schema` file from the `conf` folder of an existing Sitecore core to the folders of the cores created above.
1. Update the `WeBlog.ContentSearch.Solr.Master.config` and `WeBlog.ContentSearch.Solr.Web.config` files in the  `src/Sitecore.Modules.WeBlog/App_Config/Include/` folder to match the names of the cores created above.
1. Deploy weblog code (rebuild the solution in VS) to ensure index configs are available.
1. Use the Sitecore control panel to populate the managed schema.
1. Rebuild the indexes.

## Packaging ##

WeBlog uses Sitecore packages. A separate package is built for each minor version of Sitecore.

Before creating the package, ensure the code has been built and deployed as per above instructions, and ensure the themes have been build as well. All unit and integration tests should be run.

To create the package, copy the `WeBlog.xml` file from the `data\packages` folder over to the `App_Data\packages` folder of the Sitecore instance you'll be building from.

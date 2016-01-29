const xpath = require("xpath");
const dom = require("xmldom").DOMParser;

module.exports = function(grunt) {
	
	grunt.loadNpmTasks("grunt-contrib-less");
	grunt.loadNpmTasks("grunt-copy");
	
	grunt.initConfig({
		pkg: grunt.file.readJSON("package.json"),
		less: {
			dev: {
				files: [{
					expand: true,
					cwd: ".",
					src: ["**/*.less", "!node_modules/**/*.less", "!import_*.less"],
					dest: "build",
					ext: ".css"
				}]
			}
		},
		copy: {
			dev: {
				files: [{
					expand: true,
					cwd: "build",
					src: ["**/*.*"]
					// dest is set through deploy task
				}]
			}
		}
	});
	
	var version = grunt.option("scversion") || "sc8.0";
	
	grunt.registerTask("default", ["less", "deploy:" + version]);
	
	grunt.registerTask("deploy", "desc", function(version){
		if(!version){
			grunt.fail.warn("Missing 'version' parameter.");
		}
		
		var targetPath = resolveSitecorePath(version);
		if(!targetPath){
			grunt.fail.warn("Failed to resolve path for version '" + version + "'.");
		}
		
		grunt.config.set("copy.dev.files.0.dest", targetPath + "\\sitecore modules\\Web\\WeBlog\\Themes");
		grunt.task.run("copy:dev");
	});
	
	function resolveSitecorePath(version) {
		var file = grunt.file.read("../deploy.targets");
		var doc = new dom().parseFromString(file);

		var select = xpath.useNamespaces({
			"msb": "http://schemas.microsoft.com/developer/msbuild/2003"
		});
		
		var nodes = select("/msb:Project/msb:PropertyGroup[contains(@Condition, '" + version + "')]/msb:SitecorePath/text()", doc);
		
		return nodes[0].toString();
	};
};
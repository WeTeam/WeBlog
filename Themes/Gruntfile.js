module.exports = function(grunt) {
	
	grunt.loadNpmTasks("grunt-contrib-less");
	
	grunt.initConfig({
		pkg: grunt.file.readJSON("package.json"),
		less: {
			dev: {
				files: [{
					expand: true,
					cwd: ".",
					src: ["**/*.less", "!node_modules/**/*.less"],
					dest: "build",
					ext: ".css"
				}]
			}
		}
	});
	
	grunt.registerTask("default", ["less"]);
};
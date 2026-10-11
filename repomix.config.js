const timestamp = new Date().toISOString().slice(0, 19).replace(/[:.]/g, "-");

module.exports = {
  output: {
    filePath: `.repomix/codebase-${timestamp}.md`,
    style: "markdown",
    removeComments: false,
  },
};

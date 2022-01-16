// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
const vscode = require('vscode');
var spawn = require('child_process').spawn;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed

/**
 * @param {vscode.ExtensionContext} context
 */
function activate(context) {

	let fffout = vscode.window.createOutputChannel("FFF");

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with  registerCommand
	// The commandId parameter must match the command field in package.json
	let disposable = vscode.commands.registerCommand('fff.find', function () {
		vscode.window.showInputBox({prompt:"cmdLine"}).then(( ret)=>{
			
			fffout.show();
			fffout.clear();
			let path = '';
			if(vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length>0)
			{
				path = ' -p "'+vscode.workspace.workspaceFolders[0].uri.fsPath+'" '
				fffout.appendLine("searhing in:"+path)
			}
			var p = spawn('fff '+ret+path,{ shell: true })
			
			p.stderr.setEncoding('utf8');
			p.stderr.setEncoding('utf8');
			p.stdout.on('data', line=>{
				//fffout.appendLine(line)
			});
			p.stderr.on('data', line=>{
				 
				line = line.replace(/\x1b\[(\d+;?)+m/g,'')
				 if(/^\s+-\s/.test(line)){
					line = line.replace(/^\s+-\s/g,'')
					fffout.appendLine("\n--------------------------")
					fffout.append('"'+line+'"')
					
					fffout.append("--------------------------\n")
				}else
					fffout.appendLine(line)
			})
			
			
			
		});
	});

	context.subscriptions.push(disposable);
}

// this method is called when your extension is deactivated
function deactivate() {}

module.exports = {
	activate,
	deactivate
}

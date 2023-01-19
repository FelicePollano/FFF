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
			var results = vscode.window.createWebviewPanel("fff","FFF "+ret,vscode.ViewColumn.One,{enableScripts:true,retainContextWhenHidden: true});
			results.webview.html=getWebviewContent();

			results.webview.onDidReceiveMessage(
				message => {
				 
				  var uri = vscode.Uri.file(message.file);
				  
				  vscode.window.showTextDocument(uri,vscode.ViewColumn.Beside).then(editor=>{
					var pos1 = new vscode.Position(message.line-1,0);
					editor.selections = [new vscode.Selection(pos1,pos1)]; 
					var range = new vscode.Range(pos1, pos1);
					editor.revealRange(range);
				  });
				  
				},
				undefined,
				context.subscriptions
			  );


			let path = '';
			if(vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length>0)
			{
				path = ' -p "'+vscode.workspace.workspaceFolders[0].uri.fsPath+'" '
				fffout.appendLine("searhing in:"+path)
			}
			var p = spawn('fff -j '+ret+path,{ shell: true })
			
			p.stderr.setEncoding('utf8');
			p.stdout.setEncoding('utf8');
			var residual = '';
			p.stdout.on('data', line=>{
				var chunks = line.split(/\r?\n/);
				for(let i=0;i<chunks.length;++i){
					if(i==0){
						results.webview.postMessage(JSON.parse(residual+chunks[i]));
						residual='';
					}else
					if(i<chunks.length-1){
						results.webview.postMessage(JSON.parse(chunks[i]));
					}else{
						if(line.endsWith('\r')||line.endsWith('\r')){
							results.webview.postMessage(JSON.parse(chunks[i]));
						}else{
							residual = chunks[i];
						}
					}
				}
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
			p.on('close', (code) => {
				fffout.appendLine(`child process close all stdio with code ${code}`);
			  });
			
			
			
		});
	});

	context.subscriptions.push(disposable);
}
function getWebviewContent() {
	return `<!DOCTYPE html>
  <html lang="en">
  <head>
	  <meta charset="UTF-8">
	  <meta name="viewport" content="width=device-width, initial-scale=1.0">
	  
  </head>
  <body>
  	
	  <script>
	  const vscode = acquireVsCodeApi();
	  
	  window.addEventListener('message', event => {
        
		const find = event.data; // The JSON data our extension sent
		
		var fileDiv = document.createElement('div');
		fileDiv.setAttribute('class','fileContainer')
		var p = document.createElement('h3');
		var textNode = document.createTextNode(find.file); 
		p.appendChild(textNode);
		fileDiv.appendChild(p);
		document.body.appendChild(fileDiv);
		for (let i = 0; i <= find.findings.length; i++) {
			var p2 = document.createElement('p');
			p2.textContent=find.findings[i].LineNumber+': ';
			var a = document.createElement('a');
            a.textContent=find.findings[i].Line
			a.setAttribute('class','show-file-icons')
			fileDiv.appendChild(p2);
			p2.appendChild(a);
			var escaped = find.file;
			escaped = escaped.replace(/\\\\/g,'\\\\\\\\');
			a.setAttribute('href','javascript:void(0);')
			a.setAttribute('onclick','javascript:vscode.postMessage({file:"'+escaped+'",line:'+find.findings[i].LineNumber +'});');
		}

		
	});
	  </script>
  </body>
  </html>`;
  }
// this method is called when your extension is deactivated
function deactivate() {}

module.exports = {
	activate,
	deactivate
}

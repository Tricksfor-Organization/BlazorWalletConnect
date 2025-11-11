# Troubleshooting: Cloudflare Pages Deployment

## Common Errors and Solutions

### Error: "Project not found" (Error Code: 8000007)

**Full Error Message:**
```
Cloudflare API returned non-200: 404
API returned: {
  "result": null,
  "success": false,
  "errors": [
    {
      "code": 8000007,
      "message": "Project not found. The specified project name does not match any of your existing projects."
    }
  ],
  "messages": []
}
```

**Cause:**
The Cloudflare Pages project doesn't exist yet.

**Solution (Automatic - Recommended):**

The updated workflow (v2) automatically creates the project on first deployment. Simply retry the workflow:

1. Go to **Actions** tab in GitHub
2. Find the failed workflow run
3. Click **Re-run all jobs**
4. The workflow will create the project and deploy

**Solution (Manual):**

If automatic creation fails, create the project manually:

1. Log in to [Cloudflare Dashboard](https://dash.cloudflare.com/)
2. Navigate to **Pages** in left sidebar
3. Click **Create a project**
4. Select **Direct Upload**
5. Enter project name: `blazorwalletconnect-demo`
6. Click **Create project**
7. Re-run the GitHub Actions workflow

**Custom Project Name:**

To use a different project name:

1. Edit `.github/workflows/deploy-demo-cloudflare.yml`
2. Find line with `--project-name=blazorwalletconnect-demo`
3. Change to your desired name: `--project-name=your-custom-name`
4. Also update in the "Create project" step (search for `"name": "blazorwalletconnect-demo"`)
5. Commit and push changes

---

### Error: "Authentication error" (401)

**Error Message:**
```
Error: Authentication error
Unauthorized
```

**Cause:**
Invalid or missing Cloudflare API token.

**Solution:**

1. Verify API token is correctly added to GitHub Secrets:
   - Go to repository **Settings** → **Secrets and variables** → **Actions**
   - Check `CLOUDFLARE_API_TOKEN` exists

2. Regenerate API token:
   - Go to [Cloudflare API Tokens](https://dash.cloudflare.com/profile/api-tokens)
   - Delete old token
   - Create new token with **Cloudflare Pages Edit** permission
   - Update GitHub secret with new token

3. Verify token permissions:
   - Token must have: **Account** → **Cloudflare Pages** → **Edit**
   - Token must be scoped to your account

---

### Error: "Invalid Account ID"

**Error Message:**
```
Error: Account not found
```

**Cause:**
Incorrect Cloudflare Account ID.

**Solution:**

1. Find correct Account ID:
   - Log in to [Cloudflare Dashboard](https://dash.cloudflare.com/)
   - Click on **Pages** in left sidebar
   - Account ID is in the URL or displayed on right side
   - Format: `a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6`

2. Update GitHub secret:
   - Go to repository **Settings** → **Secrets and variables** → **Actions**
   - Edit `CLOUDFLARE_ACCOUNT_ID`
   - Paste correct Account ID

---

### Error: "Rate limit exceeded"

**Error Message:**
```
Error: Too many requests
Rate limit exceeded
```

**Cause:**
Too many API calls in short time period.

**Solution:**

1. Wait 5-10 minutes before retrying
2. Avoid triggering multiple workflows simultaneously
3. Use manual workflow dispatch instead of rapid pushes

---

### Error: "Build failed" / "Publish failed"

**Error Message:**
```
dotnet publish failed with exit code 1
```

**Cause:**
.NET build or publish errors.

**Solution:**

1. Check build logs for specific error
2. Test locally:
   ```bash
   cd demo/BlazorWalletConnectDemo
   dotnet publish --configuration Release
   ```
3. Fix any compilation errors
4. Common issues:
   - Missing dependencies: `dotnet restore`
   - Incorrect project path
   - .NET version mismatch

---

### Error: "Permission denied" / "Deployment failed"

**Error Message:**
```
Error: You do not have permission to deploy to this project
```

**Cause:**
API token lacks deployment permissions.

**Solution:**

1. Recreate API token with correct permissions:
   - **Account** → **Cloudflare Pages** → **Edit** ✓
   - Ensure account resources include your account

2. Update GitHub secret with new token

---

### Error: "Wrangler command not found"

**Error Message:**
```
bash: npx: command not found
```

**Cause:**
Node.js/npm not properly installed in workflow.

**Solution:**

Already fixed in workflow - the workflow includes:
```yaml
- name: Setup Node.js
  uses: actions/setup-node@v4
  with:
    node-version: '20.x'
```

If error persists, check workflow syntax is correct.

---

### Error: "Directory not found"

**Error Message:**
```
Error: Directory ./publish/wwwroot does not exist
```

**Cause:**
Publish step failed or used incorrect output path.

**Solution:**

1. Check "Publish demo application" step succeeded
2. Verify output directory:
   ```yaml
   run: |
     dotnet publish ... --output ./publish
   ```
3. Ensure `./publish/wwwroot` exists after publish

---

### Warning: "Base path issues" / Routes not working

**Symptoms:**
- Page loads but routes show 404
- Refresh causes 404 errors
- Navigation doesn't work

**Cause:**
Missing or incorrect `_redirects` file.

**Solution:**

Already fixed in workflow - the workflow creates `_redirects`:
```
/*    /index.html   200
```

If still having issues:
1. Check `_redirects` file exists in deployment
2. Verify Cloudflare Pages settings
3. Check browser console for errors

---

### Warning: "WebAssembly MIME type errors"

**Symptoms:**
```
wasm streaming compile failed: TypeError: Failed to execute 'compile'
Incorrect response MIME type. Expected 'application/wasm'
```

**Cause:**
Incorrect MIME types for .wasm files.

**Solution:**

Already fixed in workflow - the workflow creates `_headers`:
```
/*.wasm
  Content-Type: application/wasm
```

If still having issues:
1. Hard refresh browser (Ctrl+Shift+R)
2. Check `_headers` file in deployment
3. Wait 2-3 minutes for CDN propagation

---

### Warning: "CSS/JS MIME type errors"

**Symptoms:**
```
The resource from "https://blazorwalletconnect-demo.pages.dev/css/app.css" 
was blocked due to MIME type ("text/html") mismatch (X-Content-Type-Options: nosniff).
```

**Cause:**
Static files are being served as `text/html` instead of correct MIME type. This happens when `_redirects` catches static files.

**Solution:**

Already fixed in updated workflow - the workflow now:
1. Creates proper `_headers` with CSS/JS MIME types
2. Creates `_redirects` that excludes static file paths

The updated `_redirects` file:
```
/css/*    200
/js/*     200
/_framework/*    200
/_content/*    200
/service-worker.js    200
/*    /index.html   200
```

If still having issues:
1. Re-deploy the application
2. Hard refresh browser (Ctrl+Shift+R)
3. Clear Cloudflare cache in dashboard

---

### Warning: "Service Worker errors"

**Symptoms:**
```
ServiceWorker script at https://blazorwalletconnect-demo.pages.dev/service-worker.js 
threw an exception during script evaluation.
```

**Cause:**
Service worker file is being served as `text/html` or with wrong MIME type.

**Solution:**

Already fixed in updated workflow. The workflow now:
1. Explicitly serves `/service-worker.js` with correct MIME type
2. Excludes it from SPA redirect rules
3. Sets proper cache headers

After re-deployment:
1. Unregister old service worker in browser DevTools
2. Hard refresh (Ctrl+Shift+R)
3. Check Application → Service Workers in DevTools

---

### Error: "WalletConnect ProjectId is not configured"

**Error Message:**
```
InvalidOperationException: WalletConnect ProjectId is not configured.
```

**Cause:**
The WalletConnect Project ID is missing or not properly injected during build.

**Solution:**

1. **Add GitHub Secret:**
   - Go to repository **Settings** → **Secrets and variables** → **Actions**
   - Click **New repository secret**
   - Name: `WALLETCONNECT_PROJECT_ID`
   - Value: Your Project ID from [WalletConnect Cloud](https://cloud.walletconnect.com/)

2. **Verify workflow injects the value:**
   - Check workflow includes "Inject WalletConnect ProjectId" step
   - Workflow should update `appsettings.json` during build

3. **Get WalletConnect Project ID:**
   - Visit [WalletConnect Cloud](https://cloud.walletconnect.com/)
   - Sign up or log in
   - Create a new project
   - Copy the Project ID
   - Add it to GitHub secrets

**Note:** Cloudflare Pages environment variables won't work for Blazor WASM apps since they run in the browser. The workflow injects the ProjectId during build time instead.

---

### Deployment succeeds but site shows old version

**Symptoms:**
- Deployment shows success
- Site shows old content

**Cause:**
Browser or CDN caching.

**Solution:**

1. **Hard refresh browser:** Ctrl+Shift+R (Windows/Linux) or Cmd+Shift+R (Mac)
2. **Clear browser cache:** Browser settings → Clear cache
3. **Wait for CDN:** Cloudflare CDN can take 1-3 minutes to propagate
4. **Check Cloudflare:** Verify in Cloudflare Pages dashboard that new deployment is "Active"
5. **Purge cache:** In Cloudflare Pages → Deployments → Active deployment → "Purge cache"

---

### GitHub Actions workflow doesn't trigger on release

**Symptoms:**
- Published release but workflow didn't run

**Cause:**
Workflow file not on the branch/tag where release was created.

**Solution:**

1. Ensure workflow file is committed to `main` branch
2. Check workflow file syntax is correct
3. Verify workflow is enabled:
   - Go to **Actions** tab
   - Check workflow is not disabled
4. Check release was actually published (not draft)

---

## Debug Checklist

If deployment fails, check:

- [ ] Cloudflare API token is valid and has correct permissions
- [ ] Cloudflare Account ID is correct
- [ ] GitHub secrets are properly set (`CLOUDFLARE_API_TOKEN`, `CLOUDFLARE_ACCOUNT_ID`)
- [ ] Project name matches in workflow and Cloudflare (if manually created)
- [ ] .NET build succeeds locally
- [ ] Workflow file syntax is correct
- [ ] Release was published (not draft)
- [ ] GitHub Actions is enabled for repository

## Getting Help

If issues persist:

1. **Check GitHub Actions logs:**
   - Actions tab → Failed workflow → Expand failed step
   - Look for specific error messages

2. **Check Cloudflare logs:**
   - Cloudflare Dashboard → Pages → Your project → Deployments
   - View deployment logs

3. **Test locally:**
   ```bash
   cd demo/BlazorWalletConnectDemo
   dotnet publish --configuration Release --output ./publish
   ls -la ./publish/wwwroot  # Verify files exist
   ```

4. **Verify secrets:**
   ```bash
   # In GitHub repo
   Settings → Secrets → Actions
   # Verify both secrets exist (values are hidden, which is correct)
   ```

5. **Community support:**
   - Cloudflare Community: https://community.cloudflare.com/
   - GitHub Discussions: https://github.com/Tricksfor-Organization/BlazorWalletConnect/discussions
   - Open issue: https://github.com/Tricksfor-Organization/BlazorWalletConnect/issues

## Quick Fixes

**Most common quick fixes:**

```bash
# 1. Retry deployment (solves ~60% of issues)
GitHub Actions → Re-run workflow

# 2. Hard refresh browser (solves ~20% of issues)
Ctrl+Shift+R

# 3. Regenerate API token (solves ~15% of issues)
Cloudflare → API Tokens → Create new → Update GitHub secret

# 4. Wait and retry (solves ~5% of issues)
Wait 5-10 minutes for rate limits or propagation
```

---

**Last Updated:** Deployment workflow v2 with automatic project creation

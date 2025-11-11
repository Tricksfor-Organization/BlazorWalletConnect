# Quick Start: Deploy Demo to Cloudflare Pages

## 🚀 5-Minute Setup

### 1. Get Cloudflare Credentials

**Account ID:**
```
1. Visit https://dash.cloudflare.com/
2. Click "Pages" in sidebar
3. Copy Account ID from URL or right sidebar
```

**API Token:**
```
1. Visit https://dash.cloudflare.com/profile/api-tokens
2. Click "Create Token"
3. Use "Edit Cloudflare Workers" template
4. Add permission: Account > Cloudflare Pages > Edit
5. Create and copy the token
```

### 2. Add GitHub Secrets

```
Repository → Settings → Secrets and variables → Actions → New repository secret
```

Add these two secrets:
- `CLOUDFLARE_API_TOKEN` → Your API token from step 1
- `CLOUDFLARE_ACCOUNT_ID` → Your Account ID from step 1

### 3. Deploy

**Option A - Via Release (Recommended):**
```
1. Go to GitHub → Releases
2. Click "Draft a new release"
3. Create tag (e.g., v1.0.0)
4. Add release notes
5. Click "Publish release"
6. Workflow creates project and deploys automatically
```

**Option B - Manual:** 
```
1. Go to GitHub Actions tab
2. Select "Deploy Demo to Cloudflare Pages"
3. Click "Run workflow"
4. Project will be created automatically if needed
```

**Note:** The Cloudflare Pages project will be created automatically on first deployment.

### 4. Access Your Demo

Production: `https://blazorwalletconnect-demo.pages.dev`

## 📋 Checklist

- [ ] Cloudflare account created
- [ ] Account ID obtained
- [ ] API token created with Pages permissions
- [ ] `CLOUDFLARE_API_TOKEN` secret added to GitHub
- [ ] `CLOUDFLARE_ACCOUNT_ID` secret added to GitHub
- [ ] Workflow file committed to repository
- [ ] First deployment triggered
- [ ] Demo site accessible

## 🔧 Customization

**Change project name:**
```yaml
# Edit .github/workflows/deploy-demo-cloudflare.yml line 126
projectName: your-custom-name  # Change this
```

**Change trigger branches:**
```yaml
# Edit .github/workflows/deploy-demo-cloudflare.yml lines 7-8
branches: 
  - main  # Add more branches here
  - develop
```

## 📚 Full Documentation

See [README-CLOUDFLARE.md](./README-CLOUDFLARE.md) for detailed documentation.

## ⚡ Troubleshooting

**Deployment fails?**
1. Check GitHub Actions logs
2. Verify secrets are correctly set
3. Ensure API token has correct permissions

**404 errors on routes?**
- The `_redirects` file handles SPA routing automatically
- Check Cloudflare Pages dashboard for redirect rules

**WebAssembly loading issues?**
- The `_headers` file sets correct MIME types automatically
- Check browser console for specific errors

## 🎯 What's Deployed

The workflow:
1. ✅ Triggers on new GitHub releases
2. ✅ Builds the BlazorWalletConnect library
3. ✅ Publishes the demo Blazor WebAssembly app
4. ✅ Configures proper headers and redirects
5. ✅ Deploys to Cloudflare Pages CDN
6. ✅ Makes it available worldwide instantly
7. ✅ Includes release notes in deployment summary

## 💰 Cost

**Free tier includes:**
- ✅ Unlimited sites
- ✅ Unlimited requests
- ✅ 500 builds/month
- ✅ Global CDN
- ✅ HTTPS
- ✅ DDoS protection

Perfect for demo applications! 🎉

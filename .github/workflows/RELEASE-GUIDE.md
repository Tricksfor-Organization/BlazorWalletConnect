# Creating a Release to Deploy Demo

## Quick Steps

1. **Navigate to Releases**
   ```
   GitHub Repository → Releases → Draft a new release
   ```

2. **Create Tag**
   - Click "Choose a tag" dropdown
   - Type new tag: `v1.0.0` (or next version)
   - Click "Create new tag: v1.0.0 on publish"

3. **Fill Release Information**
   - **Release title:** e.g., "Version 1.0.0 - Initial Release"
   - **Description:** Add release notes (what's new, bug fixes, etc.)

4. **Publish**
   - Click "Publish release"
   - Deployment workflow triggers automatically
   - Demo updates within 2-5 minutes

## Version Numbering

Use [Semantic Versioning](https://semver.org/):

- `v1.0.0` - Major release (breaking changes)
- `v1.1.0` - Minor release (new features, backward compatible)
- `v1.0.1` - Patch release (bug fixes)

## Release Notes Template

```markdown
## 🎉 What's New

- New feature 1
- New feature 2
- Enhancement to existing feature

## 🐛 Bug Fixes

- Fixed issue #123
- Resolved wallet connection problem

## 🔧 Improvements

- Performance optimization
- UI/UX enhancements

## 📦 Dependencies

- Updated Nethereum to v4.x.x
- Updated WalletConnect SDK

## ⚠️ Breaking Changes

- List any breaking changes (if major version)

## 📚 Documentation

- Updated README
- Added examples
```

## Monitoring Deployment

After publishing release:

1. **Check GitHub Actions**
   - Go to Actions tab
   - Find "Deploy Demo to Cloudflare Pages" workflow
   - Watch progress (should take 2-5 minutes)

2. **Verify Deployment**
   - Check for green checkmark ✅
   - Review deployment summary
   - Click on workflow run for details

3. **Test Live Demo**
   - Visit: https://blazorwalletconnect-demo.pages.dev
   - Verify new features work
   - Check browser console for errors

## Rollback Process

If deployment has issues:

1. **Quick Rollback**
   - Go to Cloudflare Pages dashboard
   - Find previous successful deployment
   - Click "Rollback to this deployment"

2. **Fix and Re-release**
   - Fix issues in code
   - Create new patch release (e.g., v1.0.1)
   - Publish release
   - New deployment overwrites problematic one

## Pre-Release (Testing)

For testing before production:

1. Create release as usual
2. **Check "This is a pre-release"** checkbox
3. Publish pre-release
4. Deployment still triggers
5. Test thoroughly
6. Edit release and uncheck pre-release when ready

## Example Workflow

### Scenario: Deploying Version 1.2.0

```
1. Code changes merged to main branch
2. All tests passing locally
3. Ready to deploy

Steps:
→ GitHub → Releases → "Draft a new release"
→ Tag: v1.2.0
→ Title: "Version 1.2.0 - NFT Staking Support"
→ Description:
   ## What's New
   - Added NFT staking functionality
   - Improved wallet connection stability
   - Updated UI themes
   
   ## Bug Fixes
   - Fixed transaction confirmation delay
   - Resolved chain switching issue
   
→ "Publish release"
→ Wait 2-5 minutes
→ Visit https://blazorwalletconnect-demo.pages.dev
→ Test NFT staking feature
→ ✅ Success!
```

## Automated Checks

The workflow automatically:
- ✅ Builds library and demo
- ✅ Runs all tests (if configured)
- ✅ Publishes to Cloudflare
- ✅ Generates deployment summary
- ✅ Includes release notes
- ✅ Reports status

## Best Practices

1. **Always test locally first**
   ```bash
   cd demo/BlazorWalletConnectDemo
   dotnet run
   ```

2. **Write meaningful release notes**
   - Explain what changed
   - Mention breaking changes
   - Credit contributors

3. **Use consistent versioning**
   - Follow semantic versioning
   - Tag format: `v1.2.3`
   - Don't skip versions

4. **Review before publishing**
   - Check tag name
   - Review release notes
   - Verify target branch

5. **Monitor after deployment**
   - Check Actions tab
   - Test live demo
   - Watch for errors

## Troubleshooting

### Release published but workflow didn't trigger

**Check:**
- Workflow file committed to repository
- Workflow file in `.github/workflows/` directory
- GitHub Actions enabled for repository

**Solution:**
- Go to Actions tab → Find workflow → "Run workflow"
- Manually trigger deployment

### Deployment failed

**Check:**
- GitHub Actions logs for error details
- Cloudflare secrets configured correctly
- Build errors in logs

**Solution:**
- Fix identified issues
- Create new patch release
- Or manually re-run workflow

### Demo not updating

**Check:**
- Deployment completed successfully
- Browser cache (hard refresh: Ctrl+Shift+R)
- Cloudflare propagation (can take 1-2 minutes)

**Solution:**
- Wait a few minutes
- Clear browser cache
- Check Cloudflare Pages dashboard

## Quick Reference

| Action | Command/Location |
|--------|-----------------|
| Create Release | GitHub → Releases → Draft new release |
| View Deployments | GitHub → Actions → Deploy Demo workflow |
| Live Demo URL | https://blazorwalletconnect-demo.pages.dev |
| Cloudflare Dashboard | https://dash.cloudflare.com/ |
| Rollback | Cloudflare Pages → Deployments → Rollback |

## Need Help?

- **Quick Start:** [QUICKSTART-CLOUDFLARE.md](./QUICKSTART-CLOUDFLARE.md)
- **Full Guide:** [README-CLOUDFLARE.md](./README-CLOUDFLARE.md)
- **Checklist:** [DEPLOYMENT-CHECKLIST.md](./DEPLOYMENT-CHECKLIST.md)
